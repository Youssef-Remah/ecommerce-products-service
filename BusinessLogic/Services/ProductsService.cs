using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.ServiceInterfaces;
using DataAccess.Entities;
using DataAccess.RepositoryInterfaces;
using FluentValidation;
using System.Linq.Expressions;

namespace BusinessLogic.Services;

public class ProductsService(IMapper mapper,
                             IProductsRepository productsRepository,
                             IValidator<ProductAddRequest> productAddValidator,
                             IValidator<ProductUpdateRequest> productUpdateValidator): IProductsService
{
    private readonly IMapper _mapper = mapper;
    private readonly IProductsRepository _productsRepository = productsRepository;
    private readonly IValidator<ProductAddRequest> _productAddValidator = productAddValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateValidator = productUpdateValidator;

    public async Task<ProductResponse?> AddProductAsync(ProductAddRequest productRequest)
    {
        ArgumentNullException.ThrowIfNull(productRequest);

        var result = await _productAddValidator.ValidateAsync(productRequest);

        if (!result.IsValid)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }

        var product = _mapper.Map<Product>(productRequest);

        var addedProduct = await _productsRepository.AddProductAsync(product);

        if (addedProduct is null) return null;

        var response = _mapper.Map<ProductResponse>(addedProduct);

        return response;
    }

    public async Task<bool> DeleteProductAsync(Guid productID)
    {
        var product = await _productsRepository.GetSingleProductAsync(p => p.ProductID == productID);

        if (product is null) return false;

        return await _productsRepository.DeleteProductAsync(productID);
    }

    public async Task<List<ProductResponse?>> GetProductsAsync()
    {
        var products = await _productsRepository.GetProductsAsync();

        var response = _mapper.Map<IEnumerable<ProductResponse>?>(products);

        return response.ToList();
    }

    public async Task<List<ProductResponse?>> GetProductsAsync(Expression<Func<Product, bool>> predicate)
    {
        var products = await _productsRepository.GetProductsAsync(predicate);

        var response = _mapper.Map<IEnumerable<ProductResponse>?>(products);

        return response.ToList();
    }

    public async Task<ProductResponse?> GetSingleProductAsync(Expression<Func<Product, bool>> predicate)
    {
        var product = await _productsRepository.GetSingleProductAsync(predicate);

        if (product is null) return null;

        var response = _mapper.Map<ProductResponse?>(product);

        return response;
    }

    public async Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest productRequest)
    {
        var product = await _productsRepository.GetSingleProductAsync(p => p.ProductID == productRequest.ProductID);

        if (product is null) throw new ArgumentException("Invalid Product ID");

        var result = await _productUpdateValidator.ValidateAsync(productRequest);

        if (!result.IsValid)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }

        var updateResponse = _mapper.Map<Product>(productRequest);

        var response = await _productsRepository.UpdateProductAsync(updateResponse);

        var productResponse = _mapper.Map<ProductResponse?>(response);

        return productResponse;
    }
}
