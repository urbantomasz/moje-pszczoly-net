﻿using MojePszczoly.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(o => o.UpdatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey(oi => oi.OrderItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
