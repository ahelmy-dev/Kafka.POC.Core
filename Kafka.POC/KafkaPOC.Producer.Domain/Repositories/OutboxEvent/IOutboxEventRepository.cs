
namespace KafkaPOC.Producer.Domain.Repositories.OutboxEvent
{
    public interface IOutboxEventRepository : IRepository<Entities.OutboxEvent.OutboxEvent>
    {
        Task<List<Entities.OutboxEvent.OutboxEvent>> GetUnpublishedAsync(int limit);
    }
}
