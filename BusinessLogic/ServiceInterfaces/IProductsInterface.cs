using BusinessLogic.DTOs;
using DataAccess.Entities;
using System.Linq.Expressions;

namespace BusinessLogic.ServiceInterfaces;

public interface IProductsInterface
{
    Task<List<ProductResponse?>> GetProductsAsync();

    Task<List<ProductResponse?>> GetProductsAsync(Expression<Func<Product, bool>> predicate);

    Task<ProductResponse?> GetSingleProductAsync(Expression<Func<Product, bool>> predicate);

    Task<ProductResponse?> AddProductAsync(ProductAddRequest productRequest);

    Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest productRequest);

    Task<bool> DeleteProductAsync(Guid productID);
}
