using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("Promotions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.DiscountPercentage)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.Property(p => p.EndDate)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);
        builder.Property(p => p.Description)
            .HasMaxLength(500);
        builder.Property(p => p.CustomerLevel)
            .IsRequired();
        builder.Property(p => p.Usage)
            .IsRequired();
            
        builder.HasMany(p => p.Products)
            .WithMany(p => p.Promotions)
            .UsingEntity(j => j.ToTable("PromotionProducts"));

        builder.HasMany(p => p.Orders)
            .WithOne(o => o.Promotion)
            .HasForeignKey(o => o.PromotionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
