using Scalar.AspNetCore;
using MySqlConnector;
using Microsoft.AspNetCore.HttpOverrides;

namespace BlogApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Forwarded Headers beállítása a Render reverse proxy támogatásához (HTTPS felismerése)
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

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

            // Forwarded Headers middleware használata a kéréskezelési folyamat legelején
            app.UseForwardedHeaders();

            // 2. CORS middleware bekapcsolása (Fontos: az UseAuthorization / MapControllers ELŐTT legyen!)
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            // MapOpenApi és MapScalarApiReference engedélyezése minden környezetben (Production-ben is)
            app.MapOpenApi();
            app.MapScalarApiReference();

            // app.UseHttpsRedirection(); // Renderen a reverse proxy intézi a HTTPS-t

            app.UseAuthorization();

            app.UseStaticFiles(); // Ez engedélyezi az index.html és egyéb statikus fájlok betöltését

            app.MapControllers();

            app.Run();
        }
    }
}
