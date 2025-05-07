using GroceryDeliveryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GroceryDeliveryAPI.Context
{
    public class GroceryDeliveryContext : DbContext
    {
        public GroceryDeliveryContext(DbContextOptions<GroceryDeliveryContext> options)
        : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
       .HasDiscriminator<string>("Discriminator")
       .HasValue<User>("User")
       .HasValue<DeliveryPerson>("DeliveryPerson");

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>();
            // Product to Category (many-to-one)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Order to User (many-to-one)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // OrderItem to Order (many-to-one)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem to Product (many-to-one)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            // Delivery to Order (many-to-one)
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Order)
                .WithMany(o => o.Deliveries)
                .HasForeignKey(d => d.OrderId);

            // Delivery to DeliveryPerson (many-to-one)
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.DeliveryPerson)
                .WithMany(dp => dp.Deliveries)
                .HasForeignKey(d => d.DeliveryPersonId);
        }
    }
}
