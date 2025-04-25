using System;
using DearHome_Backend.Services.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using DearHome_Backend.Services.Implementations;
using DearHome_Backend.Models;

namespace DearHome_Backend.Helpers;

public static class ServiceExtensions
{
    public static void AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAttributeService, AttributeService>();
        services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();
        services.AddScoped<IPlacementService, PlacementService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAttributeService, AttributeService>();
        services.AddScoped<IVariantService, VariantService>();
        services.AddScoped<IGoodReceivedNoteService, GoodReceivedNoteService>();
        services.AddScoped<IAddressRepository, AddressRepository>();
    }

    public static void AddScopedRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAttributeRepository, AttributeRepository>();
        services.AddScoped<IPlacementRepository, PlacementRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAttributeValueRepository, AttributeValueRepository>();
        services.AddScoped<IVariantRepository, VariantRepository>();
        services.AddScoped<IVariantAttributeRepository, VariantAttributeRepository>();
        services.AddScoped<IGoodReceivedNoteRepository, GoodReceivedNoteRepository>();
        services.AddScoped<IGoodReceivedItemRepository, GoodReceivedItemRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
    }
}
