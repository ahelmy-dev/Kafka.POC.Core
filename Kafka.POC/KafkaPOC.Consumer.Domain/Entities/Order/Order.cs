
namespace KafkaPOC.Consumer.Domain.Entities.Order
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string CustomerName { get; private set; }
        public Guid OrderId { get; private set; }
        public DateTime CreationDate { get; private set; }

        public Order(Guid id, string customerName,Guid orderId)
        {
            Id = id;
            OrderId = orderId;
            CustomerName = customerName;
            CreationDate = DateTime.UtcNow;
        }
        private Order() { }
    }
}