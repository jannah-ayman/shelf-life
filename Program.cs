using Microsoft.EntityFrameworkCore;
using ShelfLife.Models;
using ShelfLife.Repository;
using ShelfLife.Repository.Base;

namespace ShelfLife
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<DBcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Myconection")));

            // REPOSITORIES
            // Base generic repository
            builder.Services.AddTransient(typeof(Irepo<>), typeof(MainRepository<>));

            // Specific repositories
            builder.Services.AddScoped<IBookListingRepository, BookListingRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            builder.Services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
            builder.Services.AddScoped<IRatingRepository, RatingRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            // CORS policy (if needed for frontend)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

            var app = builder.Build();

            // Seed default categories on startup
            using (var scope = app.Services.CreateScope())
            {
                var categoryRepo = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
                await categoryRepo.SeedDefaultCategoriesAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shelf Life API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            // Enable static files for image serving
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            // Enable CORS
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}