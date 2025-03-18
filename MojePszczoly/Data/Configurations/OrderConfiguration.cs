using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MojePszczoly.Data.Models;

namespace MojePszczoly.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.OrderId);

            builder.Property(o => o.CustomerName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(o => o.Note)
                 .HasMaxLength(200);

            builder.Property(o => o.Phone)
                   .IsRequired()
                   .HasMaxLength(9);

            builder.Property(o => o.OrderDate)
                   .IsRequired();

            builder.Property(o => o.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(o => o.UpdatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
