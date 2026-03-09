using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MojePszczoly.Infrastructure.Entities;

namespace MojePszczoly.Infrastructure.Configurations
{
    public class DateOverrideConfiguration : IEntityTypeConfiguration<DateOverride>
    {
        public void Configure(EntityTypeBuilder<DateOverride> builder)
        {
            builder.HasKey(d => d.DateOverrideId);

            builder.Property(d => d.Date)
                   .IsRequired();

            builder.Property(d => d.IsAdded)
                   .IsRequired();

            builder.Property(d => d.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(d => d.Date)
                   .IsUnique();
        }
    }
}
