using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Models;

namespace MojePszczoly.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.OrderItemId);

            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            builder.HasOne(oi => oi.Bread)
                   .WithMany()
                   .HasForeignKey(oi => oi.BreadId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Order>()
                 .WithMany(o => o.Items)
                 .HasForeignKey(oi => oi.OrderItemId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
