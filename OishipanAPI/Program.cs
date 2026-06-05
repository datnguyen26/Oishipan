using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using OishipanAPI.Services;
using CloudinaryDotNet;
using Oishipan.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using OishipanAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Basic API info
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Oishipan API",
        Version = "v1",
        Description = "Oishipan API - Swagger documentation"
    });

    // JWT Bearer token support in Swagger
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [bearerScheme] = new string[] { }
    });

    // Include XML comments if available
    try
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
    }
    catch
    {
        // ignore missing xml
    }
});

// Database configuration
var connectionString = Environment.GetEnvironmentVariable("OISHIPAN_DB_CONNECTION")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=.;Database=Oishipan;Trusted_Connection=true;TrustServerCertificate=true;";

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
builder.Services.AddScoped<IUserService, UserService>();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
}
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Register JWT token generator
builder.Services.AddSingleton<JwtTokenGenerator>();



// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .WithOrigins(
                "http://localhost:5200",
                "https://localhost:5201",
                "http://localhost:5202",
                "http://localhost:8080",
                "http://127.0.0.1:5200",
                "https://127.0.0.1:5201",
                "http://127.0.0.1:5202"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OishipanContext>();
    try
    {
        // Check if database can connect
        if (context.Database.CanConnect())
        {
            Console.WriteLine("✓ Database connection successful");

            if (DatabaseMigrationHelper.HasLegacySchema(context) && !DatabaseMigrationHelper.MigrationHistoryTableExists(context))
            {
                Console.WriteLine("✓ Legacy schema detected, initializing EF migration history");
                DatabaseMigrationHelper.EnsureLegacySchemaCompatibility(context);
                DatabaseMigrationHelper.EnsureMigrationHistory(context, context.Database.GetMigrations());
            }

            // Get pending migrations
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Count > 0)
            {
                Console.WriteLine($"Applying {pendingMigrations.Count} pending migration(s)...");
                context.Database.Migrate();
                Console.WriteLine("✓ Migrations applied successfully");
            }
            else
            {
                Console.WriteLine("✓ No pending migrations");
            }
        }
        else
        {
            Console.WriteLine("Database connection failed, attempting to create...");
            context.Database.EnsureCreated();
            Console.WriteLine("✓ Database created");
        }
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already an object named"))
    {
        // Migration conflict: table already exists, skip migration
        Console.WriteLine($"⚠ Migration skipped: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Database error: {ex.Message}");
        try
        {
            context.Database.EnsureCreated();
            Console.WriteLine("✓ Database created via EnsureCreated");
        }
        catch (Exception ex2)
        {
            Console.WriteLine($"✗ Failed to create database: {ex2.Message}");
            throw;
        }
    }

    // Seed admin account if doesn't exist or if existing admin is inactive
    try
    {
        const string defaultAdminEmail = "admin@oishipan.com";
        var activeAdminExists = context.Accounts.Any(a => a.UserRole == Role.Admin && a.Status);
        var existingAdminAccount = context.Accounts.FirstOrDefault(a => a.Email == defaultAdminEmail);

        if (!activeAdminExists)
        {
            if (existingAdminAccount == null)
            {
                context.Accounts.Add(new Oishipan.Models.Account
                {
                    FullName = "Oishipan Admin",
                    Email = defaultAdminEmail,
                    PhoneNumber = "0123456789",
                    Password = PasswordHelper.HashPassword("Admin@123"),
                    UserRole = Role.Admin,
                    Address = "Văn phòng Oishipan",
                    Status = true
                });

                context.SaveChanges();
                Console.WriteLine("✓ Admin account created");
            }
            else
            {
                existingAdminAccount.UserRole = Role.Admin;
                existingAdminAccount.Status = true;
                if (string.IsNullOrWhiteSpace(existingAdminAccount.FullName))
                    existingAdminAccount.FullName = "Oishipan Admin";
                if (string.IsNullOrWhiteSpace(existingAdminAccount.PhoneNumber))
                    existingAdminAccount.PhoneNumber = "0123456789";
                if (string.IsNullOrWhiteSpace(existingAdminAccount.Address))
                    existingAdminAccount.Address = "Văn phòng Oishipan";

                context.Accounts.Update(existingAdminAccount);
                context.SaveChanges();
                Console.WriteLine("✓ Existing admin account restored and activated");
            }
        }
        else
        {
            Console.WriteLine("✓ Active admin account already exists");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error seeding admin: {ex.Message}");
    }

    // Fix plain text passwords (convert to bcrypt if needed)
    try
    {
        var accountsWithPlainPasswords = context.Accounts
            .Where(a => a.Password != null && !a.Password.StartsWith("$2"))
            .ToList();

        if (accountsWithPlainPasswords.Count > 0)
        {
            Console.WriteLine($"⚠ Found {accountsWithPlainPasswords.Count} account(s) with plain text password. Converting to bcrypt...");
            foreach (var account in accountsWithPlainPasswords)
            {
                var plainPassword = account.Password;
                account.Password = PasswordHelper.HashPassword(plainPassword);
                context.Accounts.Update(account);
                Console.WriteLine($"✓ Password hashed for {account.Email}");
            }
            context.SaveChanges();
            Console.WriteLine("✓ All plain text passwords converted to bcrypt");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error fixing passwords: {ex.Message}");
    }
}

// THÊM DÒNG NÀY VÀO ĐÂY (Phải nằm trên các app.Use khác)
app.UseMiddleware<OishipanAPI.Middlewares.ExceptionMiddleware>();

// Configure the HTTP request pipeline
// Enable Swagger JSON in all environments
app.UseSwagger();

// Enable Swagger UI in all environments (can be restricted to Development for production security)
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oishipan API v1.0");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Oishipan API Documentation";
    c.DefaultModelsExpandDepth(2);
    c.DisplayOperationId();
});

// Disable HTTPS redirection in development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Add a root endpoint
app.MapGet("/", () => Results.Json(new { message = "Oishipan API is running!", docs = "/swagger" }));

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
