using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MojePszczoly.Data.Models;

namespace MojePszczoly.Data.Configurations
{
    public class BreadConfiguration : IEntityTypeConfiguration<Bread>
    {
        public void Configure(EntityTypeBuilder<Bread> builder)
        {
            builder.HasKey(b => b.BreadId);

            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.ShortName)
                   .HasMaxLength(50);

            builder.Property(b => b.SortOrder)
                   .HasDefaultValue(0);
        }
    }
}
