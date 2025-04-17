
namespace KafkaPOC.Producer.Application.DTOs
{
    public class OutboxEventLogDto
    {
        public Guid EventId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string Payload { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
    }
}
