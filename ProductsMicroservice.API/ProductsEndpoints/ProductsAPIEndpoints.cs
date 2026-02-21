using BusinessLogic.DTOs;
using BusinessLogic.ServiceInterfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicroservice.API.ProductsEndpoints;

public static class ProductsAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        //GET /api/products
        app.MapGet("/api/products", async (IProductsService productService) =>
        {
            var products = await productService.GetProductsAsync();

            return Results.Ok(products);
        });

        //GET /api/products/search/product-id/{id}
        app.MapGet("/api/products/search/product-id/{id:guid}", async (IProductsService productService, Guid id) =>
        {
            var product = await productService.GetSingleProductAsync(p => p.ProductID == id);

            return Results.Ok(product);
        });

        //GET /api/products/search/{search-filter}
        app.MapGet("/api/products/search/{filter}", async (IProductsService productService, string filter) =>
        {
            //var productsByName = await productService.GetProductsAsync(p => p.ProductName != null &&
            //                                  p.ProductName.Contains(filter, StringComparison.OrdinalIgnoreCase));
            var productsByName = await productService.GetProductsAsync(p => p.ProductName != null &&
                                              EF.Functions.Like(p.ProductName, $"%{filter}%"));

            var productsByCategory = await productService.GetProductsAsync(p => p.Category != null &&
                                              EF.Functions.Like(p.Category, $"%{filter}%"));

            var products = productsByName.Union(productsByCategory);

            return Results.Ok(products);
        });

        //POST /api/products
        app.MapPost("/api/products", async (IProductsService productService,
                                            ProductAddRequest request,
                                            IValidator<ProductAddRequest> productAddValidator) =>
        {
            var validationResult = await productAddValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.GroupBy(e => e.PropertyName)
                                       .ToDictionary(grp => grp.Key, grp => grp.Select(err => err.ErrorMessage).ToArray());
                
                return Results.ValidationProblem(errors);
            }

            var product = await productService.AddProductAsync(request);

            if (product != null) return Results.Created($"/api/products/search/product-id/{product.ProductID}", product);

            return Results.Problem("Error while adding product");
        });

        //PUT /api/products
        app.MapPut("/api/products", async (IProductsService productService,
                                            ProductUpdateRequest request,
                                            IValidator<ProductUpdateRequest> productUpdateValidator) =>
        {
            var validationResult = await productUpdateValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.GroupBy(e => e.PropertyName)
                                       .ToDictionary(grp => grp.Key, grp => grp.Select(err => err.ErrorMessage).ToArray());

                return Results.ValidationProblem(errors);
            }

            var product = await productService.UpdateProductAsync(request);

            if (product != null) return Results.Ok(product);

            return Results.Problem("Error while updating product");
        });

        //DELETE /api/products/{id}
        app.MapDelete("/api/products/{id:guid}", async (IProductsService productService, Guid id) =>
        {
            var isDeleted = await productService.DeleteProductAsync(id);

            if (isDeleted) return Results.Ok(true);

            return Results.Problem("Error while deleting product");
        });

        return app;
    }
}
