

namespace KafkaPOC.Producer.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
