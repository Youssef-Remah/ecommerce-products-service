using DataAccess.Context;
using DataAccess.Repositories;
using DataAccess.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!
                                            .Replace("$MYSQL_HOST", Environment.GetEnvironmentVariable("MYSQL_HOST"))
                                            .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"))
                                            .Replace("$MYSQL_PORT", Environment.GetEnvironmentVariable("MYSQL_PORT"))
                                            .Replace("$MYSQL_DATABASE", Environment.GetEnvironmentVariable("MYSQL_DATABASE"))
                                            .Replace("$MYSQL_USER", Environment.GetEnvironmentVariable("MYSQL_USER"));


        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySQL(connectionString);
        });

        services.AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}
