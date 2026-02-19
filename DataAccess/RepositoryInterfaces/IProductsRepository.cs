using DataAccess.Entities;
using System.Linq.Expressions;

namespace DataAccess.RepositoryInterfaces;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();

    Task<IEnumerable<Product>> GetProductsAsync(Expression<Func<Product, bool>> predicate);

    Task<Product?> GetSingleProductAsync(Expression<Func<Product, bool>> predicate);

    Task<Product> AddProductAsync(Product product);

    Task<Product?> UpdateProductAsync(Product product);

    Task<bool> DeleteProductAsync(Guid productID);
}
