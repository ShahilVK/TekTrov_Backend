
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using TekTrov.Application.DTOs;
using TekTrov.Application.Interfaces.Repositories;
using TekTrov.Application.Interfaces.Services;
using TekTrov.Application.Services;
using TekTrov.Infrastructure.Data;
using TekTrov.Infrastructure.Repositories;
using TekTrov.Infrastructure.Services;
using CloudinaryDotNet;
using TekTrov.Application.Common;


var builder = WebApplication.CreateBuilder(args);

// --------------------
// Controllers & Swagger
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --------------------
// Database
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// --------------------
// JWT Settings
// --------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// --------------------
// Services
// --------------------
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IImageService, CloudinaryImageService>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddScoped<IPaymentService, RazorpayPaymentService>();
builder.Services.Configure<RazorpaySettings>(
    builder.Configuration.GetSection("Razorpay"));





// --------------------
// Authentication (JWT)
// --------------------
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(options =>
//{
//    var jwtSettings = builder.Configuration
//        .GetSection("Jwt")
//        .Get<JwtSettings>()
//        ?? throw new InvalidOperationException("JWT settings missing");

//    var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;

//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),

//        ValidateIssuer = true,
//        ValidIssuer = jwtSettings.Issuer,

//        ValidateAudience = true,
//        ValidAudience = jwtSettings.Audience,

//        ValidateLifetime = true,
//        ClockSkew = TimeSpan.Zero,

//        // 🔑 CRITICAL FIX
//        NameClaimType = ClaimTypes.NameIdentifier,

//        RoleClaimType = ClaimTypes.Role
//    };


//    // ✅ Allow JWT from cookie if needed
//    options.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.ContainsKey("accessToken"))
//            {
//                context.Token = context.Request.Cookies["accessToken"];
//            }
//            return Task.CompletedTask;
//        },
//        OnTokenValidated = async context =>
//        {
//            var userRepo = context.HttpContext.RequestServices
//                .GetRequiredService<IUserRepository>();

//            var userIdClaim = context.Principal?
//                .FindFirst(ClaimTypes.NameIdentifier);

//            if (userIdClaim == null)
//            {
//                context.Fail("Invalid token");
//                return;
//            }

//            var userId = int.Parse(userIdClaim.Value);
//            var user = await userRepo.GetByIdAsync(userId);

//            // 🚫 BLOCKED USER HANDLING
//            if (user == null || user.IsBlocked)
//            {
//                context.Fail("User is blocked by admin");
//            }
//        },


//        OnChallenge = context =>
//        {
//            context.HandleResponse();

//            var message =
//                context.ErrorDescription != null &&
//                context.ErrorDescription.ToLower().Contains("blocked")
//                    ? "Your account has been blocked"
//                    : "Invalid or missing token";

//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            context.Response.ContentType = "application/json";

//            return context.Response.WriteAsync(
//                System.Text.Json.JsonSerializer.Serialize(new
//                {
//                    statusCode = 401,
//                    message = message,
//                    data = (object?)null
//                })
//            );
//        },

//        OnForbidden = context =>
//        {
//            context.Response.StatusCode = StatusCodes.Status403Forbidden;
//            context.Response.ContentType = "application/json";

//            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
//            {
//                statusCode = 403,
//                message = "You do not have permission to access this resource",
//                data = (object?)null
//            }));
//        }
//    };
//});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration
        .GetSection("Jwt")
        .Get<JwtSettings>()
        ?? throw new InvalidOperationException("JWT settings missing");

    var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;

            if (path.StartsWithSegments("/api/auth/refresh"))
            {
                context.NoResult();
                return Task.CompletedTask;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                context.Token = authHeader.Substring("Bearer ".Length).Trim();
            }

            return Task.CompletedTask;
        },

        OnTokenValidated = async context =>
        {
            var userRepo = context.HttpContext.RequestServices
                .GetRequiredService<IUserRepository>();

            var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                context.Fail("Invalid token");
                return;
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await userRepo.GetByIdAsync(userId);

            if (user == null || user.IsBlocked)
            {
                context.Fail("User is blocked by admin");
            }
        },

        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    statusCode = 401,
                    message = "Invalid or expired access token",
                    data = (object?)null
                })
            );
        }
    };
});


// --------------------
// Authorization
// --------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});




builder.Services.AddDistributedMemoryCache();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // React dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});




var cloudinarySettings = builder.Configuration
    .GetSection("Cloudinary")
    .Get<CloudinarySettings>()
    ?? throw new Exception("Cloudinary settings missing");


var account = new Account(
    cloudinarySettings.CloudName,
    cloudinarySettings.ApiKey,
    cloudinarySettings.ApiSecret
);



var cloudinary = new Cloudinary(account)
{
    Api = { Secure = true }
};

builder.Services.AddSingleton(cloudinary);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await context.Database.MigrateAsync();
    await DbInitializer.SeedAdminAsync(context);
}


app.UseCors("AllowReactApp");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();



app.UseHttpsRedirection();


app.MapControllers();
app.Run();