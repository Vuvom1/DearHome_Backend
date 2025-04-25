using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class VariantAttributeConfiguration : IEntityTypeConfiguration<VariantAttribute>
{
    public void Configure(EntityTypeBuilder<VariantAttribute> builder)
    {
        builder.ToTable("VariantAttributes");

        builder.HasKey(v => v.Id);

        builder.HasKey(va => va.Id)
            .HasName("PK_VariantAttributes");
        
        builder.HasOne(va => va.Variant)
            .WithMany(v => v.VariantAttributes)
            .HasForeignKey(va => va.VariantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(va => va.AttributeValue)
            .WithMany(av => av.VariantAttributes)
            .HasForeignKey(va => va.AttributeValueId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
