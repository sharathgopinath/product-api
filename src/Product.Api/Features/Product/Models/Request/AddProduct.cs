namespace Product.Api.Features.Product.Models.Request;

public record AddProduct(
    int Id,
    string Title,
    string? Description,
    decimal Price,
    decimal DiscountPercentage,
    decimal? Rating,
    int? Stock,
    string? Brand,
    string? Category,
    string? Thumbnail,
    List<string>? Images);
