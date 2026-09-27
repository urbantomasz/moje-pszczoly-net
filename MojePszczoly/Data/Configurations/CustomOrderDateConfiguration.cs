using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MojePszczoly.Data.Models;

namespace MojePszczoly.Data.Configurations
{
    public class CustomOrderDateConfiguration : IEntityTypeConfiguration<CustomOrderDate>
    {
        public void Configure(EntityTypeBuilder<CustomOrderDate> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DateUtc)
                   .IsRequired();

            builder.Property(d => d.Note)
                   .HasMaxLength(200);

            builder.HasIndex(d => d.DateUtc)
                   .IsUnique();
        }
    }
}
