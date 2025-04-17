
using KafkaPOC.Consumer.Domain.Entities.Order;
using KafkaPOC.Consumer.Domain.Entities.ProcessedEvent;
using Microsoft.EntityFrameworkCore;

namespace KafkaPOC.Consumer.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<ProcessedEvent>().ToTable("ProcessedEvents");

            base.OnModelCreating(modelBuilder);
        }
    }
}