using KafkaPOC.Producer.Application.DTOs;

namespace KafkaPOC.Producer.Application.Interfaces.OutboxEventLog
{
    public interface IOutboxEventLogRepository
    {

        Task AddAsync(OutboxEventLogDto log);
        Task<List<OutboxEventLogDto>> GetByEventIdAsync(Guid orderId);
    }
}