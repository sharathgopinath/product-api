using DataModels = global::Product.Data.Models;
using Request = Product.Api.Features.Product.Models.Request;
using Response = Product.Api.Features.Product.Models.Response;

namespace Product.Api.Features.Product.Mappers;

public static class ProductMapperExtensions
{
    public static Response.Product MapToResponse(this DataModels.Product product) =>
        new()
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            DiscountPercentage = product.DiscountPercentage,
            Rating = product.Rating,
            Stock = product.Stock,
            Brand = product.Brand,
            Category = product.Category,
            Thumbnail = product.Thumbnail,
            Images = [.. product.Images]
        };

    public static List<Response.Product> MapToResponse(this IEnumerable<DataModels.Product> products) =>
        products.Select(p => p.MapToResponse()).ToList();

    public static DataModels.Product MapToData(this Request.AddProduct request) =>
        new()
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            DiscountPercentage = request.DiscountPercentage,
            Rating = request.Rating,
            Stock = request.Stock,
            Brand = request.Brand,
            Category = request.Category,
            Thumbnail = request.Thumbnail,
            Images = [.. request.Images ?? []]
        };

    public static DataModels.Product MapToData(this Request.UpdateProduct request) =>
        new()
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            DiscountPercentage = request.DiscountPercentage,
            Rating = request.Rating,
            Stock = request.Stock,
            Brand = request.Brand,
            Category = request.Category,
            Thumbnail = request.Thumbnail,
            Images = [.. request.Images ?? []]
        };
}
