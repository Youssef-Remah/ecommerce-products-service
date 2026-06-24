using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.RabbitMQ;
using BusinessLogic.ServiceInterfaces;
using DataAccess.Entities;
using DataAccess.RepositoryInterfaces;
using FluentValidation;
using System.Linq.Expressions;

namespace BusinessLogic.Services;

public class ProductsService : IProductsService
{
    private readonly IMapper _mapper;
    private readonly IProductsRepository _productsRepository;
    private readonly IValidator<ProductAddRequest> _productAddValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateValidator;
    private readonly IRabbitMQPublisher _rabbitMQPublisher;

    public ProductsService(IMapper mapper,
                           IProductsRepository productsRepository,
                           IValidator<ProductAddRequest> productAddValidator,
                           IValidator<ProductUpdateRequest> productUpdateValidator,
                           IRabbitMQPublisher rabbitMQPublisher)
    {
        _mapper = mapper;
        _productsRepository = productsRepository;
        _productAddValidator = productAddValidator;
        _productUpdateValidator = productUpdateValidator;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

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

        var isDeleted = await _productsRepository.DeleteProductAsync(productID);

        if (isDeleted)
        {
            var message = new ProductDeletionMessage(product.ProductID, product.ProductName);
            var routingKey = "product.delete";
            _rabbitMQPublisher.Publish(routingKey, message);
        }

        return isDeleted;
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

        bool isProductNameChanged = productRequest.ProductName != product.ProductName;

        var response = await _productsRepository.UpdateProductAsync(updateResponse);

        if (isProductNameChanged)
        {
            var routingKey = "product.update.name";
            var message = new ProductNameUpdateMessage(response.ProductID, response.ProductName);
            _rabbitMQPublisher.Publish(routingKey, message);
        }

        var productResponse = _mapper.Map<ProductResponse?>(response);

        return productResponse;
    }
}
