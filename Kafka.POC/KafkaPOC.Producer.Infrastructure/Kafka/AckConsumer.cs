using Confluent.Kafka;
using KafkaPOC.Producer.Application.DTOs;
using KafkaPOC.Producer.Infrastructure.Entities;
using KafkaPOC.Producer.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace KafkaPOC.Producer.Infrastructure.Kafka
{
    public class AckConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AckConsumer> _logger;

        public AckConsumer(IServiceScopeFactory scopeFactory, ILogger<AckConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "ack-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                };

                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe("order-ack-topic");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                        if (cr?.Message == null)
                            continue;

                        var orderAck = JsonConvert.DeserializeObject<OrderDto>(cr.Message.Value);

                        var eventId = Guid.Parse(orderAck.Id.ToString());
                        var customerName = orderAck.CustomerName.ToString();
                        var orderId = Guid.Parse(orderAck.Id.ToString());


                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        _logger.LogInformation("Raw ACK value: {Value}", cr.Message.Value);
                        var evt = dbContext.OutboxEvents.FirstOrDefault(e => e.Id == eventId);
                        if (evt == null)
                        {
                            _logger.LogWarning("No OutboxEvent found for ACK ID: {EventId}", eventId);
                        }
                        else
                        {
                            _logger.LogInformation("Found OutboxEvent. IsAcknowledged: {IsAck}", evt.IsAcknowledged);
                        }
                        if (evt != null && !evt.IsAcknowledged)
                        {
                            evt.MarkAcknowledged();

                            var log = new OutboxEventLog
                            {
                                EventId = eventId,
                                Payload = evt.Payload,
                                CustomerName = evt.CustomerName,
                                OrderId = evt.OrderId,
                                CreationDate = DateTime.UtcNow
                            };

                            var retryPolicy = Policy
                                .Handle<Exception>()
                                .WaitAndRetry(3, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)),
                                    (exception, timeSpan, retryCount, _) =>
                                    {
                                        log.Status = "Retry";
                                        log.Reason = $"Retry {retryCount}: {exception.Message}";
                                        dbContext.OutboxEventLogs.Add(log);
                                        dbContext.SaveChanges();
                                        _logger.LogWarning("Retry {Retry} saving ACK for {EventId}: {Message}",
                                            retryCount, eventId, exception.Message);
                                    });

                            try
                            {
                                retryPolicy.Execute(() =>
                                {
                                    log.Status = "Acknowledged Received";
                                    log.Reason = null;
                                    dbContext.OutboxEventLogs.Add(log);
                                    dbContext.SaveChanges();
                                    _logger.LogInformation("ACK saved for event: {EventId}", eventId);
                                });
                            }
                            catch (Exception ex)
                            {
                                log.Status = "FailedAck";
                                log.Reason = $"Final failure: {ex.Message}";
                                dbContext.OutboxEventLogs.Add(log);
                                dbContext.SaveChanges();
                                _logger.LogError(ex, "Final failure saving ACK for {EventId}", eventId);
                            }
                        }
                        else if (evt == null)
                        {
                            var log = new OutboxEventLog
                            {
                                EventId = eventId,
                                CustomerName = customerName,
                                OrderId = orderId,
                                Status = "FailedAck",
                                Reason = "ACK received for unknown event",
                                Payload = string.Empty,
                                CreationDate = DateTime.UtcNow
                            };

                            dbContext.OutboxEventLogs.Add(log);
                            dbContext.SaveChanges();

                            _logger.LogWarning("ACK received for unknown event: {EventId}", eventId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing ACK message.");
                    }
                }

                consumer.Close();
            }, stoppingToken);
        }
    }
}