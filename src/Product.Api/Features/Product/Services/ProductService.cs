using FluentValidation;
using Product.Api.Exceptions;
using Product.Api.Features.Product.Mappers;
using Product.Data;
using Product.Api.Features.Product.Models.Request;
using Request = Product.Api.Features.Product.Models.Request;
using Response = Product.Api.Features.Product.Models.Response;

namespace Product.Api.Features.Product.Services;

public class ProductService(
    IValidator<AddProduct> addProductValidator,
    IValidator<UpdateProduct> updateProductValidator,
    IValidator<Request.GetProducts> getProductsValidator,
    IValidator<Request.GetProduct> getProductValidator,
    ProductRepository productRepository)
{
    public async Task<Response.GetProducts> GetProducts(Request.GetProducts request)
    {
        await getProductsValidator.ValidateAndThrowAsync(request);

        var (products, totalCount) = await productRepository.GetProducts(request.PageSize, request.PageNumber);
        var responseProducts = products.MapToResponse();
        return new Response.GetProducts
        {
            Products = responseProducts,
            TotalCount = totalCount
        };
    }

    public async Task<Response.Product> GetProduct(Request.GetProduct request)
    {
        await getProductValidator.ValidateAndThrowAsync(request);

        var product = await productRepository.GetProduct(request.Id)
                      ?? throw new NotFoundException($"Product with id {request.Id} was not found.");

        return product.MapToResponse();
    }

    public async Task<int> AddProduct(Request.AddProduct request)
    {
        await addProductValidator.ValidateAndThrowAsync(request);

        if (!await productRepository.AddProduct(request.MapToData()))
            throw new ConflictException($"Product with id {request.Id} already exists.");

        return request.Id;
    }

    public async Task UpdateProduct(Request.UpdateProduct request)
    {
        await updateProductValidator.ValidateAndThrowAsync(request);

        if (!await productRepository.UpdateProduct(request.MapToData()))
            throw new NotFoundException($"Product with id {request.Id} was not found.");
    }
}