using KafkaPOC.Producer.Domain.Repositories.Order;
using KafkaPOC.Producer.Domain.Repositories.OutboxEvent;
using KafkaPOC.Producer.Domain.Repositories;
using KafkaPOC.Producer.Infrastructure.Persistence;
using KafkaPOC.Producer.Infrastructure.Repositories.Order;
using KafkaPOC.Producer.Infrastructure.Repositories.OutboxEvent;

namespace KafkaPOC.Producer.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IOrderRepository Orders { get; }
        public IOutboxEventRepository OutboxEvents { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Orders = new OrderRepository(context);
            OutboxEvents = new OutboxEventRepository(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
