using Domain.Abstractions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IWebinarRepository, WebinarRepository>();

            services.AddScoped<IUnitOfWork>(
            factory => factory.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IDbConnection>(
                factory => factory.GetRequiredService<ApplicationDbContext>().Database.GetDbConnection());
        }
    }
}
