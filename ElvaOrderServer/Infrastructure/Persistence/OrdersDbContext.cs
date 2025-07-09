using ElvaOrderServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElvaOrderServer.Infrastructure.Persistence
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).ValueGeneratedOnAdd();

                entity.Property(o => o.ExternalOrderId).IsRequired();
                entity.Property(o => o.OrderId).IsRequired();
                entity.Property(o => o.CustomerId).IsRequired();
                entity.Property(o => o.CreatedAt).IsRequired();

                entity.HasIndex(o => o.ExternalOrderId).IsUnique();
                entity.HasIndex(o => o.OrderId).IsUnique();

                entity.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Id).ValueGeneratedOnAdd();
                entity.Property(i => i.ProductId).IsRequired();
                entity.Property(i => i.Quantity).IsRequired();
            });

            SeedData(modelBuilder);

        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1000000001L,
                    ExternalOrderId = Guid.Parse("a3e8f1b2-4c6d-4e5f-9a0b-1c2d3e4f5a6b"),
                    OrderId = 2000000001L,
                    CustomerId = 3000000001L,
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = 1000000002L,
                    ExternalOrderId = Guid.Parse("7d8e9f0a-1b2c-3d4e-5f6a-7b8c9d0e1f2a"),
                    OrderId = 2000000002L,
                    CustomerId = 3000000002L,
                    CreatedAt = DateTime.UtcNow
                }
            );


            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = 5000000001L,
                    OrderId = 1000000001L,
                    ProductId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                    Quantity = 2
                },
                new OrderItem
                {
                    Id = 5000000002L,
                    OrderId = 1000000001L,
                    ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                    Quantity = 1
                },
                new OrderItem
                {
                    Id = 5000000003L,
                    OrderId = 1000000002L,
                    ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                    Quantity = 3
                }
            );
        }
    }
}
