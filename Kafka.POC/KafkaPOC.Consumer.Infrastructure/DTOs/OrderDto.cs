
namespace KafkaPOC.Consumer.Infrastructure.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public Guid OrderId { get; set; }
    }
}
