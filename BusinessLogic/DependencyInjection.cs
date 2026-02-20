using BusinessLogic.Mappers;
using BusinessLogic.ServiceInterfaces;
using BusinessLogic.Services;
using BusinessLogic.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProductAddMappingProfile).Assembly);

        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();

        services.AddScoped<IProductsService, ProductsService>();

        return services;
    }
}
