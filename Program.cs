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