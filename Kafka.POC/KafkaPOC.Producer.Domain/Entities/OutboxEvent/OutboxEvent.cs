namespace KafkaPOC.Producer.Domain.Entities.OutboxEvent
{
    public class OutboxEvent : Entity<Guid>
    {
        #region Props
        public string EventType { get; private set; }
        public string CustomerName { get; private set; }
        public Guid OrderId { get; private set; }
        public string Payload { get; private set; }
        public bool IsPublished { get; private set; }
        public int RetryCount { get; private set; } = 0;
        public bool IsAcknowledged { get; private set; }
        #endregion

        #region Ctor
        public OutboxEvent(string eventType, string payload,Guid orderId , string customerName)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            CustomerName = customerName;
            EventType = eventType;
            Payload = payload;
            CreationDate = DateTime.UtcNow;
        }
        #endregion

        public void SetPayload(string payload)
        {
            Payload = payload;
        }
        public void MarkPublished() => IsPublished = true;
        public void MarkAcknowledged() => IsAcknowledged = true;
        public void IncrementRetry() => RetryCount++;
    }
}
