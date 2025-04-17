using Confluent.Kafka;
using KafkaPOC.Consumer.Domain.Entities.Order;
using KafkaPOC.Consumer.Domain.Entities.ProcessedEvent;
using KafkaPOC.Consumer.Infrastructure.DTOs;
using KafkaPOC.Consumer.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace KafkaPOC.Consumer.Infrastructure.Kafka
{
    public class OrderConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderConsumer> _logger;

        public OrderConsumer(IServiceScopeFactory scopeFactory, ILogger<OrderConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "order-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                };

                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe("orders-topic");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                        if (cr == null || cr.Message == null)
                            continue;

                        var message = cr.Message.Value;
                        var orderDto = JsonConvert.DeserializeObject<OrderDto>(message);
                        var order = new Order(orderDto.Id, orderDto.CustomerName,orderDto.OrderId);
                        var eventId = order.Id.ToString();

                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var alreadyProcessed = dbContext.ProcessedEvents.Any(e => e.EventId == eventId);

                        if (!alreadyProcessed)
                        {
                            dbContext.Orders.Add(order);
                            dbContext.ProcessedEvents.Add(new ProcessedEvent(eventId));
                            dbContext.SaveChanges();

                            consumer.Commit(cr);
                            _logger.LogInformation("Order processed: {OrderId}", order.Id);

                            var ackConfig = new ProducerConfig
                            {
                                BootstrapServers = "localhost:9092"
                            };

                            var messagePayload = JsonConvert.SerializeObject(order);

                            using var ackProducer = new ProducerBuilder<Null, string>(ackConfig).Build();
                            await ackProducer.ProduceAsync("order-ack-topic", new Message<Null, string>
                            {
                                Value = messagePayload
                            });

                            _logger.LogInformation("Acknowledgment sent for event: {EventId}", eventId);
                        }
                        else
                        {
                            _logger.LogInformation("Duplicate event skipped: {OrderId}", order.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error consuming Kafka message.");
                    }
                }

                consumer.Close();
            }, stoppingToken);
        }
    }
}