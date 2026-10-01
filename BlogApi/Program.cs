
using Scalar.AspNetCore;
using MySqlConnector;

namespace BlogApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 1. CORS szolgáltatás hozzáadása
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddHttpClient();

            var app = builder.Build();
            // 2. CORS middleware bekapcsolása (Fontos: az UseAuthorization / MapControllers ELŐTT legyen!)
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles(); // Ez engedélyezi az index.html és egyéb statikus fájlok betöltését

            app.MapControllers();

            app.Run();
        }
    }
}
