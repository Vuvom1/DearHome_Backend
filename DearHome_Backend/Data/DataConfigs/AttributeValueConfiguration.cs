using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class AttributeValueConfiguration : IEntityTypeConfiguration<Models.AttributeValue>
{
    public void Configure(EntityTypeBuilder<Models.AttributeValue> builder)
    {
        builder.HasKey(av => av.Id);

        builder.Property(av => av.Value)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(av => av.Attribute)
            .WithMany(a => a.AttributeValues)
            .HasForeignKey(av => av.AttributeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(av => av.VariantAttributes)
            .WithOne(va => va.AttributeValue)
            .HasForeignKey(va => va.AttributeValueId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
