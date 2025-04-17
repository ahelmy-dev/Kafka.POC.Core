using KafkaPOC.Producer.Domain.Repositories.Order;
using KafkaPOC.Producer.Domain.Repositories.OutboxEvent;

namespace KafkaPOC.Producer.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IOutboxEventRepository OutboxEvents { get; }
        Task<int> CommitAsync();
    }
}
