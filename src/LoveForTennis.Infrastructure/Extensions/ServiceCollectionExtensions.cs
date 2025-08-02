using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LoveForTennis.Infrastructure.Data;

namespace LoveForTennis.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, string applicationName)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (!string.IsNullOrEmpty(connectionString))
        {
            // Use SQL Server if connection string is provided
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
        else
        {
            // Fallback to InMemory database if no connection string
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase($"LoveForTennisDb-{applicationName}-{Guid.NewGuid()}"));
        }
        
        return services;
    }
}