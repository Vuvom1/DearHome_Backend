using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class AttributeConfiguration : IEntityTypeConfiguration<Models.Attribute>
{
    public void Configure(EntityTypeBuilder<Models.Attribute> builder)
    {
        builder.ToTable("Attributes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Type)
            .IsRequired();

        builder.HasMany(a => a.CategoryAttributes)
            .WithOne(ca => ca.Attribute)
            .HasForeignKey(ca => ca.AttributeId);

        builder.HasMany(a => a.AttributeValues)
            .WithOne(av => av.Attribute)
            .HasForeignKey(av => av.AttributeId)
            .OnDelete(DeleteBehavior.Cascade); // Optional: Set delete behavior if needed
    }
}
