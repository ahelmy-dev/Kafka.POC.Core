namespace KafkaPOC.Producer.Domain.Repositories.Order
{
    public interface IOrderRepository : IRepository<Entities.Order.Order>
    {
        Task<(List<Entities.Order.Order> Orders, int TotalCount)> GetPagedAsync(
                string? customerName, int page, int pageSize, CancellationToken cancellationToken);
    }
}
