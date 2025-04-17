using KafkaPOC.Producer.Domain.Repositories.Order;
using KafkaPOC.Producer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KafkaPOC.Producer.Infrastructure.Repositories.Order
{
    public class OrderRepository : Repository<Domain.Entities.Order.Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<(List<Domain.Entities.Order.Order> Orders, int TotalCount)> GetPagedAsync(
     string? customerName, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(o => o.CustomerName.Contains(customerName));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderByDescending(o => o.CreationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (orders, totalCount);
        }
    }
}
