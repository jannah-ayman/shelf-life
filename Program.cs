using Microsoft.EntityFrameworkCore;
using ShelfLife.Models;
using ShelfLife.Repository;
using ShelfLife.Repository.Base;
namespace ShelfLife
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<DBcontext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Myconection")));
            // REPOS
            builder.Services.AddTransient(typeof(Irepo<>) , typeof(MainRepository<>));
            builder.Services.AddScoped<BookListingRepository>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
