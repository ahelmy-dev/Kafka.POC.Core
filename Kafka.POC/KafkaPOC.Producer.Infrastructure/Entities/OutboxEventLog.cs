
namespace KafkaPOC.Producer.Infrastructure.Entities
{
    public class OutboxEventLog
    {
        public int Id { get; set; }
        public Guid EventId { get; set; }
        public string CustomerName { get; set; }
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty; 
        public string? Reason { get; set; }
        public string Payload { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
