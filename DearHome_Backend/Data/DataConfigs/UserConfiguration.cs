using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DearHome_Backend.Data.DataConfigs;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.ImageUrl).HasMaxLength(200);
        builder.Property(u => u.DateOfBirth).HasColumnType("date");
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.CustomerPoints).HasDefaultValue(0);
        builder.Property(u => u.CustomerLevel).HasDefaultValue(CustomerLevels.Bronze);
    }
}
