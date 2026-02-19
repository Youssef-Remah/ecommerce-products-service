using DataAccess.Context;
using DataAccess.Entities;
using DataAccess.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return product;
    }

    public async Task<bool> DeleteProductAsync(Guid productID)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productID);

        if (product is not null)
        {
            _dbContext.Products.Remove(product);
            int rowsAffected = await _dbContext.SaveChangesAsync();

            if(rowsAffected > 0) return true;
        }

        return false;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(Expression<Func<Product, bool>> predicate)
    {
        return await _dbContext.Products.Where(predicate).ToListAsync();
    }

    public async Task<Product?> GetSingleProductAsync(Expression<Func<Product, bool>> predicate)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(predicate);
    }

    public async Task<Product?> UpdateProductAsync(Product product)
    {
        var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductID == product.ProductID);

        if (existingProduct is not null)
        {
            existingProduct.ProductName = product.ProductName;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.QuantityInStock = product.QuantityInStock;
            existingProduct.Category = product.Category;

            await _dbContext.SaveChangesAsync();
        }

        return existingProduct;
    }
}
