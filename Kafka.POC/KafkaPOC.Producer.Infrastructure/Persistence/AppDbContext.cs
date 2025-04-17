using KafkaPOC.Producer.Domain.Entities.Order;
using KafkaPOC.Producer.Domain.Entities.OutboxEvent;
using KafkaPOC.Producer.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace KafkaPOC.Producer.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }
        public DbSet<OutboxEventLog> OutboxEventLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OutboxEvent>().ToTable("OutboxEvents");
            modelBuilder.Entity<OutboxEventLog>().ToTable("OutboxEventLogs");

            base.OnModelCreating(modelBuilder);
        }
    }
}
