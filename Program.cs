using Microsoft.EntityFrameworkCore;
using ShelfLife.Models;
using ShelfLife.Repository;
using ShelfLife.Repository.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace ShelfLife
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- Add DbContext
            builder.Services.AddDbContext<DBcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Myconection")));

            // --- Add Repositories
            builder.Services.AddTransient(typeof(Irepo<>), typeof(MainRepository<>));
            builder.Services.AddScoped<IBookListingRepository, BookListingRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            builder.Services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            // --- Controllers
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            // --- CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // --- Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Shelf Life API",
                    Version = "v1",
                    Description = "API for the Shelf Life book recycling platform"
                });
            });

            // --- JWT Authentication (must be BEFORE builder.Build())
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // DEBUG: Log authentication events
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                        Console.WriteLine($"Claims: {string.Join(", ", claims ?? Array.Empty<string>())}");
                        return Task.CompletedTask;
                    }
                };
            });

            // Add authorization policy that checks userId in route matches JWT claim
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UserMatchesRoute", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var httpContext = context.Resource as HttpContext;
                        if (httpContext == null)
                            return false;

                        var routeUserId = httpContext.Request.RouteValues["userId"]?.ToString();
                        var claimUserId = context.User.FindFirst("userId")?.Value;

                        return routeUserId == claimUserId;
                    }));
            });

            // Add this after the existing "UserMatchesRoute" policy in Program.cs

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UserMatchesRoute", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var httpContext = context.Resource as HttpContext;
                        if (httpContext == null)
                            return false;

                        var routeUserId = httpContext.Request.RouteValues["userId"]?.ToString();
                        var claimUserId = context.User.FindFirst("userId")?.Value;

                        return routeUserId == claimUserId;
                    }));

                // NEW: Add policy for Business users only
                options.AddPolicy("BusinessOnly", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var userType = context.User.FindFirst(ClaimTypes.Role)?.Value;
                        return userType == "BUSINESS";
                    }));

                // NEW: Add policy for Normal users only
                options.AddPolicy("NormalUserOnly", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var userType = context.User.FindFirst(ClaimTypes.Role)?.Value;
                        return userType == "NORMAL_USER";
                    }));
            });

            var app = builder.Build();

            // --- Seed default categories
            using (var scope = app.Services.CreateScope())
            {
                var categoryRepo = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
                await categoryRepo.SeedDefaultCategoriesAsync();
            }

            // --- Middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shelf Life API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");

            app.UseAuthentication(); // must be before Authorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}