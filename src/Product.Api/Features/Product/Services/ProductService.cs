using FluentValidation;
using Product.Api.Features.Product.Models.Request;
using Request = Product.Api.Features.Product.Models.Request;
using Response = Product.Api.Features.Product.Models.Response;

namespace Product.Api.Features.Product.Services;

public class ProductService(
    IValidator<AddProduct> addProductValidator,
    IValidator<UpdateProduct> updateProductValidator)
{
    public async Task<Response.GetProducts> GetProducts()
    {
        return new();
    }

    public async Task<int> AddProduct(Request.AddProduct request)
    {
        await addProductValidator.ValidateAndThrowAsync(request);

        return 0;
    }

    public async Task UpdateProduct(Request.UpdateProduct request)
    {
        await updateProductValidator.ValidateAndThrowAsync(request);

        return;
    }
}