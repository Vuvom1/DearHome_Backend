using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");

        builder.HasKey(od => od.Id);

        builder.Property(od => od.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(od => od.UnitPrice)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(od => od.TotalPrice)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(od => od.Variant)
            .WithMany(v => v.OrderDetails)
            .HasForeignKey(od => od.VariantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(od => od.Reviews)
            .WithOne(r => r.OrderDetail)
            .HasForeignKey(r => r.OrderDetailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
