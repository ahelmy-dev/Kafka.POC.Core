
using KafkaPOC.Producer.Application.DTOs;
using KafkaPOC.Producer.Application.Interfaces.OutboxEventLog;
using KafkaPOC.Producer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KafkaPOC.Producer.Infrastructure.Repositories.OutboxEventLog
{
    public class OutboxEventLogRepository : IOutboxEventLogRepository
    {
        private readonly AppDbContext _context;

        public OutboxEventLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OutboxEventLogDto dto)
        {
            Entities.OutboxEventLog log = new  Entities.OutboxEventLog
            {
                EventId = dto.EventId,
                Status = dto.Status,
                Reason = dto.Reason,
                Payload = dto.Payload,
                CreationDate = dto.CreationDate
            };

            await _context.OutboxEventLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        

        public async Task<List<OutboxEventLogDto>> GetByEventIdAsync(Guid orderId)
        {
            var outboxLogs = await _context.OutboxEventLogs
                .Where(l => l.OrderId == orderId)
                .OrderByDescending(l => l.CreationDate).ToListAsync();

            return await _context.OutboxEventLogs
                .Where(l => l.OrderId == orderId)
                .OrderByDescending(l => l.CreationDate)
                .Select(l => new OutboxEventLogDto
                {
                    EventId = l.EventId,
                    Status = l.Status,
                    Reason = l.Reason,
                    Payload = l.Payload,
                    CreationDate = l.CreationDate
                })
                .ToListAsync();
        }
    }
}