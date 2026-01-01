
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TekTrov.Domain.Entities;

namespace ECommerce.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
           .Property(oi => oi.Price)
           .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
       .Property(o => o.TotalAmount)
       .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

            base.OnModelCreating(modelBuilder);


        }
    }
}
