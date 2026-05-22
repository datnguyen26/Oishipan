using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using OishipanAPI.Services;
using CloudinaryDotNet;
using Oishipan.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("DefaultConnection is not configured in appsettings.json");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection cannot be empty");
}

builder.Services.AddDbContext<OishipanContext>(options =>
    options.UseSqlServer(connectionString));



// Cloudinary configuration
var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var cloudName = cloudinarySettings["CloudName"] ?? throw new InvalidOperationException("Cloudinary CloudName is not configured");
var apiKey = cloudinarySettings["ApiKey"] ?? throw new InvalidOperationException("Cloudinary ApiKey is not configured");
var apiSecret = cloudinarySettings["ApiSecret"] ?? throw new InvalidOperationException("Cloudinary ApiSecret is not configured");

var cloudinary = new Cloudinary(new CloudinaryDotNet.Account(
    cloudName,
    apiKey,
    apiSecret
));
builder.Services.AddSingleton(cloudinary);

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();



// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OishipanContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed, falling back to EnsureCreated(): {ex.Message}");
        context.Database.EnsureCreated();
    }

    if (!context.Accounts.Any(a => a.Role == "Admin"))
    {
        context.Accounts.Add(new Oishipan.Models.Account
        {
            FullName = "Oishipan Admin",
            Email = "admin@oishipan.com",
            PhoneNumber = "0123456789",
            Password = PasswordHelper.HashPassword("Admin@123"),
            Role = "Admin",
            Address = "Văn phòng Oishipan",
            Status = true
        });

        context.SaveChanges();
    }
}

// THÊM DÒNG NÀY VÀO ĐÂY (Phải nằm trên các app.Use khác)
app.UseMiddleware<OishipanAPI.Middlewares.ExceptionMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Disable HTTPS redirection in development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

// Add a root endpoint
app.MapGet("/", () => Results.Json(new { message = "Oishipan API is running!", openapi = "/openapi/v1.json" }));

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }
    Environment.Exit(1);
}
