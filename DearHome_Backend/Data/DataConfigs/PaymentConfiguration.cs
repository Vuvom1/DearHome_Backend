using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CardHolderName)
            .IsRequired();

        builder.Property(p => p.CardNumber)
            .IsRequired();

        builder.Property(p => p.ExpirationDate)
            .IsRequired();

        builder.Property(p => p.Method)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.CardType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
