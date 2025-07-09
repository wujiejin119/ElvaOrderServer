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
                entity.Property(o => o.CustomerId).IsRequired();
                entity.Property(o => o.CreatedAt).IsRequired();

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

            var order1Id = new Guid("a8b3c7d0-9e1f-4a6b-8c3d-2e5f4a6b8c9d");
            var order2Id = new Guid("b9c4d8e1-0f2a-5b7c-9d3e-1f6a7b8c9d0e");

            var customer1Id = new Guid("c1d2e3f4-5a6b-7c8d-9e0f-1a2b3c4d5e6f");
            var customer2Id = new Guid("d3e4f5a6-7b8c-9d0e-1f2a-3b4c5d6e7f8a");

            var product1Id = new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479");
            var product2Id = new Guid("550e8400-e29b-41d4-a716-446655440000");
            var product3Id = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = order1Id,
                    CustomerId = customer1Id,
                    CreatedAt = new DateTime(2023, 6, 15, 10, 30, 0, DateTimeKind.Utc)
                },
                new Order
                {
                    Id = order2Id,
                    CustomerId = customer2Id,
                    CreatedAt = new DateTime(2023, 6, 16, 14, 45, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<OrderItem>().HasData(

                new OrderItem
                {
                    Id = 1,
                    OrderId = order1Id,
                    ProductId = product1Id,
                    Quantity = 2
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = order1Id,
                    ProductId = product2Id,
                    Quantity = 1
                },

                new OrderItem
                {
                    Id = 3,
                    OrderId = order2Id,
                    ProductId = product2Id,
                    Quantity = 3
                },
                new OrderItem
                {
                    Id = 4,
                    OrderId = order2Id,
                    ProductId = product3Id,
                    Quantity = 1
                }
            );

        }
    }
}
