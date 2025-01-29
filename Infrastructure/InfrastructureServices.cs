using Application.Abstractions.Authentication;
using Domain.Abstractions;
using Infrastructure.Authentication;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IWebinarRepository, WebinarRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtProvider, JwtProvider>();

            services.AddScoped<IUnitOfWork>(
            factory => factory.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IDbConnection>(
                factory => factory.GetRequiredService<ApplicationDbContext>().Database.GetDbConnection());

            //Bu satır, appsettings.json'daki JWT ayarlarını JwtOptions sınıfına baglamak icin kullanılıyor. Ancak bu islemi yapabilmek icin Microsoft.Extensions.Options paketine ihtiyacimiz var.
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            services.AddScoped<IJwtProvider, JwtProvider>();
        }
    }
}
