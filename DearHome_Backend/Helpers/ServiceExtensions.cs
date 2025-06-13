using System;
using DearHome_Backend.Services.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using DearHome_Backend.Services.Implementations;
using DearHome_Backend.Models;
using DearHome_Backend.Services;

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
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IStatisticService, StatisticService>();
        services.AddSingleton<NatsService>();
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
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
    }

    public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IShippingService, ShippingService>(client =>
        {
            client.BaseAddress = new Uri(configuration["Goship:BaseUrl"]!);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });
        services.AddHttpClient<IPaymentService, PaymentService>(client =>
        {
            client.BaseAddress = new Uri(configuration["PayOS:BaseUrl"]!);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });
    }
}
