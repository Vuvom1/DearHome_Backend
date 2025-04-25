using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.TotalPrice).IsRequired();
        builder.Property(o => o.ShippingPrice).IsRequired();
        builder.Property(o => o.Discount).IsRequired();
        builder.Property(o => o.Tax).IsRequired();
        builder.Property(o => o.FinalPrice).IsRequired();
        builder.Property(o => o.PromotionID).IsRequired();
        builder.Property(o => o.Note).HasMaxLength(500);
        builder.Property(o => o.TrackingNumber).HasMaxLength(100);
        builder.Property(o => o.DeliveryDate).IsRequired(false);
        builder.Property(o => o.AddressId).IsRequired();

        builder.HasOne(o => o.Address)
            .WithMany(a => a.Orders)
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Promotion)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PromotionID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Payment)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PaymentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(o => o.PaymentMethod).IsRequired();

        builder.Property(o => o.Status).IsRequired();

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
