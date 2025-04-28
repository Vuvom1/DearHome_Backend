using DearHome_Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DearHome_Backend.Helpers;
using DearHome_Backend.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using DearHome_Backend.Middlewares;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Text.Json;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.AllowTrailingCommas = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<DearHomeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information),
    ServiceLifetime.Scoped);

builder.Services.AddIdentity<User, IdentityRole<Guid>>() // Use IdentityRole<Guid>
    .AddEntityFrameworkStores<DearHomeContext>()
    .AddDefaultTokenProviders();

ServiceExtensions.AddScopedServices(builder.Services);
ServiceExtensions.AddScopedRepositories(builder.Services);
ServiceExtensions.AddHttpClients(builder.Services, builder.Configuration);


builder.Services.AddApplicationInsightsTelemetry(options => {
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = true;
    options.EnableQuickPulseMetricStream = true;
});

// Enable dependency tracking for HttpClient
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => {
    module.EnableSqlCommandTextInstrumentation = true;
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var firebaseCredentialPath = "Credentials/makemyhome-27df4-firebase-adminsdk-hsal7-09e697d0ff.json";

if (FirebaseApp.DefaultInstance == null)
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseCredentialPath)
    });
}

// Development environment: Use in-memory cache
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
// Production environment: Use Azure Redis Cache
else
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
        options.InstanceName = "DearHome_";
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

try
{
    using (var connection = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
    {
        connection.Open();
        Console.WriteLine("Database connection successful!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
}

app.UseMiddleware<ErrorHandlerMiddleware.GlobalErrorHandlerMiddleware>();

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
