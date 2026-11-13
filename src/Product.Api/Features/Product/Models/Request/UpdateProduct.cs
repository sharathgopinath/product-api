namespace Product.Api.Features.Product.Models.Request;

public record UpdateProduct(
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
