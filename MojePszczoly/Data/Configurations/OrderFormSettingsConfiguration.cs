using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MojePszczoly.Data.Models;

namespace MojePszczoly.Data.Configurations
{
    public class OrderFormSettingsConfiguration : IEntityTypeConfiguration<OrderFormSettings>
    {
        public void Configure(EntityTypeBuilder<OrderFormSettings> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .ValueGeneratedNever();

            builder.Property(s => s.UseCustomDates)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(s => s.NoticeEnabled)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(s => s.NoticeTitle)
                   .HasMaxLength(120);

            builder.Property(s => s.NoticeMessage)
                   .HasMaxLength(500);

            builder.Property(s => s.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasData(new OrderFormSettings
            {
                Id = 1,
                UseCustomDates = false,
                NoticeEnabled = false,
                NoticeTitle = null,
                NoticeMessage = null,
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
