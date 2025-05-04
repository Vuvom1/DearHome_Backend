using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DearHome_Backend.Models;
using DearHome_Backend.Modals;
using DearHome_Backend.Constants;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DearHome_Backend.Data.DataConfigs;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace DearHome_Backend.Data;

public class DearHomeContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DearHomeContext(DbContextOptions<DearHomeContext> options)
        : base(options)
    {
    }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Models.Attribute> Attributes { get; set; }
    public DbSet<AttributeValue> AttributeValues { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
    public DbSet<Combo> Combos { get; set; }
    public DbSet<ProductCombo> ProductCombos { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<GoodReceivedNote> GoodReceivedNotes { get; set; }
    public DbSet<GoodReceivedItem> GoodReceivedItems { get; set; }
    public DbSet<Variant> Variants { get; set; }
    public DbSet<VariantAttribute> VariantAttributes { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Placement> Placements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Example: modelBuilder.ApplyConfiguration(new UserConfiguration());

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VariantConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new VariantAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new VariantConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var adminId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid> { Id = adminRoleId, Name = UserRole.Admin.ToString(), NormalizedName = UserRole.Admin.ToString().ToUpper() },
            new IdentityRole<Guid> { Id = userRoleId, Name = UserRole.User.ToString(), NormalizedName = UserRole.User.ToString().ToUpper() }
        );

        modelBuilder.Entity<User>().HasData(
            new User 
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                Name = "Admin User",
                PhoneNumber = "1234567890",
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin@123"),
            }
        );

        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                UserId = adminId,
                RoleId = adminRoleId
            }
        );
    }
}
