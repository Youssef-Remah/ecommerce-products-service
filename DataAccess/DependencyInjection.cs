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
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!);
        });

        services.AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}
