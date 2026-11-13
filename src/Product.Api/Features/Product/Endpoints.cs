using Microsoft.AspNetCore.Http.HttpResults;
using Response = Product.Api.Features.Product.Models.Response;

namespace Product.Api.Features.Product;

public static class Endpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("product").WithTags("Product");

        group.MapGet("list", () =>
            TypedResults.Ok());
    }
}
