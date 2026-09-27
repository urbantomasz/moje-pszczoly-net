using Microsoft.EntityFrameworkCore;
using MojePszczoly.Data.Configurations;
using MojePszczoly.Data.Models;

namespace MojePszczoly.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Bread> Breads { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderFormSettings> OrderFormSettings { get; set; }
        public virtual DbSet<CustomOrderDate> CustomOrderDates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BreadConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderFormSettingsConfiguration());
            modelBuilder.ApplyConfiguration(new CustomOrderDateConfiguration());
        }
    }
}
