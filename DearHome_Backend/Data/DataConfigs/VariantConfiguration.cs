using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.ToTable("Variants");

        builder.HasKey(v => v.Id);

        builder.HasMany(v => v.VariantAttributes)
            .WithOne(va => va.Variant)
            .HasForeignKey(va => va.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(v => v.Reviews)
            .WithOne(r => r.Variant)
            .HasForeignKey(r => r.VariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
