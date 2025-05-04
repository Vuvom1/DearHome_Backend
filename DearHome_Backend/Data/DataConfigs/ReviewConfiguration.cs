using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.ReviewText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.HasOne(r => r.OrderDetail)
            .WithMany(od => od.Reviews)
            .HasForeignKey(r => r.OrderDetailId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
