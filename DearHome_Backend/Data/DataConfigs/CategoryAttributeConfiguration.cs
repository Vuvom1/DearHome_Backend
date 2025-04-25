using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class CategoryAttributeConfiguration : IEntityTypeConfiguration<Models.CategoryAttribute>
{
    public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
    {
        builder.HasKey(ca => ca.Id);

        builder.HasOne(ca => ca.Category)
            .WithMany(c => c.CategoryAttributes)
            .HasForeignKey(ca => ca.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ca => ca.Attribute)
            .WithMany(a => a.CategoryAttributes)
            .HasForeignKey(ca => ca.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ca => ca.CategoryId)
            .IsRequired();

        builder.Property(ca => ca.AttributeId)
            .IsRequired();
    }
}
