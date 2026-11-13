using Product.Api.Features.Product.Services;
using Request = Product.Api.Features.Product.Models.Request;

namespace Product.Api.Features.Product;

public static class Endpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("product").WithTags("Product");

        group.MapGet("list", async ([AsParameters] Request.GetProducts request, ProductService productService) =>
            TypedResults.Ok(await productService.GetProducts(request)));

        group.MapGet("", async ([AsParameters] Request.GetProduct request, ProductService productService) =>
            TypedResults.Ok(await productService.GetProduct(request)));

        group.MapPost("add", async (Request.AddProduct request, ProductService productService) =>
            TypedResults.Ok(await productService.AddProduct(request)));

        group.MapPost("update", async (Request.UpdateProduct request, ProductService productService) =>
        {
            await productService.UpdateProduct(request);
            return TypedResults.NoContent();
        });
    }
}
