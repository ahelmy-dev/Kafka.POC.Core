using KafkaPOC.Producer.Domain.Repositories.OutboxEvent;
using KafkaPOC.Producer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KafkaPOC.Producer.Infrastructure.Repositories.OutboxEvent
{
    public class OutboxEventRepository : Repository<Domain.Entities.OutboxEvent.OutboxEvent>, IOutboxEventRepository
    {
        public OutboxEventRepository(AppDbContext context) : base(context) { }

        public async Task<List<Domain.Entities.OutboxEvent.OutboxEvent>> GetUnpublishedAsync(int limit)
        {
            return await _context.OutboxEvents
                .Where(e => !e.IsPublished && e.RetryCount < 5)
                .OrderBy(e => e.CreationDate)
                .Take(limit)
                .ToListAsync();
        }
    }
}
