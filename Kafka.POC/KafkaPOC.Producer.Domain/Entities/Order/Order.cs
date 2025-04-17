namespace KafkaPOC.Producer.Domain.Entities.Order
{
    public class Order : Entity<Guid>
    {
        #region Props
        public string CustomerName { get; private set; }
        #endregion

        #region Ctor
        public Order(string customerName)
        {
            Id = Guid.NewGuid();
            CustomerName = customerName;
            CreationDate = DateTime.UtcNow;
        }
        #endregion

    }
}
