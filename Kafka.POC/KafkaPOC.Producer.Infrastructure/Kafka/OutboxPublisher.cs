using Confluent.Kafka;
using KafkaPOC.Producer.Infrastructure.Persistence;
using KafkaPOC.Producer.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using KafkaPOC.Producer.Domain.Entities.OutboxEvent;

namespace KafkaPOC.Producer.Infrastructure.Kafka
{
    public class OutboxPublisher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxPublisher> _logger;
        private readonly IProducer<Null, string> _kafkaProducer;

        public OutboxPublisher(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxPublisher> logger,
            IProducer<Null, string> kafkaProducer)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _kafkaProducer = kafkaProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var events = await dbContext.OutboxEvents
                    .Where(e => !e.IsPublished)
                    .OrderBy(e => e.CreationDate)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var evt in events)
                {
                    var retryPolicy = Policy
                        .Handle<ProduceException<Null, string>>()
                        .Or<Exception>()
                        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                            onRetry: (ex, delay, attempt, _) =>
                            {
                                _logger.LogWarning($"Kafka retry {attempt} for event {evt.Id}: {ex.Message}");

                                dbContext.OutboxEventLogs.Add(new OutboxEventLog
                                {
                                    EventId = evt.Id,
                                    CustomerName = evt.CustomerName,
                                    OrderId = evt.OrderId,
                                    Status = "Retry",
                                    Reason = ex.Message,
                                    Payload = evt.Payload,
                                    CreationDate = DateTime.UtcNow
                                });
                                evt.IncrementRetry();
                                dbContext.SaveChanges();
                            });

                    try
                    {
                        var payload = $"{{\"Id\":\"{evt.Id}\",\"CustomerName\":\"{evt.CustomerName}\",\"OrderId\":\"{evt.OrderId}\"}}";
                        evt.SetPayload(payload);

                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            var tcs = new TaskCompletionSource<DeliveryResult<Null, string>>();

                            _kafkaProducer.Produce("orders-topic", new Message<Null, string> { Value = evt.Payload }, result =>
                            {
                                if (result.Status == PersistenceStatus.Persisted)
                                    tcs.SetResult(result);
                                else
                                    tcs.SetException(new Exception($"Kafka delivery failed: {result.Status}"));
                            });

                            await tcs.Task;

                            evt.MarkPublished();

                            dbContext.OutboxEventLogs.Add(new OutboxEventLog
                            {
                                EventId = evt.Id,
                                CustomerName = evt.CustomerName,
                                OrderId = evt.OrderId,
                                Status = "Sent",
                                Reason = null,
                                Payload = evt.Payload,
                                CreationDate = DateTime.UtcNow
                            });

                            await dbContext.SaveChangesAsync(stoppingToken);
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Kafka publish failed for event {evt.Id}");

                        evt.IncrementRetry();

                        dbContext.OutboxEventLogs.Add(new OutboxEventLog
                        {
                            EventId = evt.Id,
                            OrderId = evt.OrderId,
                            CustomerName = evt.CustomerName,
                            Status = "Failed",
                            Reason = ex.Message,
                            Payload = evt.Payload,
                            CreationDate = DateTime.UtcNow
                        });

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}