
namespace KafkaPOC.Consumer.Domain.Entities.ProcessedEvent
{
    public class ProcessedEvent
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string EventId { get; private set; }
        public DateTime ProcessedDate { get; private set; } = DateTime.UtcNow;

        public ProcessedEvent(string eventId)
        {
            EventId = eventId;
        }
        private ProcessedEvent() { }
    }
}