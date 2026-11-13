using Microsoft.AspNetCore.Http.HttpResults;
using Request = Product.Api.Features.Product.Models.Request;
using Response = Product.Api.Features.Product.Models.Response;

namespace Product.Api.Features.Product;

public static class Endpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("product").WithTags("Product");

        group.MapGet("list", () =>
            TypedResults.Ok());

        group.MapPost("add", (Request.AddProduct request) =>
            TypedResults.Ok());

        group.MapPost("update", (Request.UpdateProduct request) =>
            TypedResults.Ok());
    }
}
