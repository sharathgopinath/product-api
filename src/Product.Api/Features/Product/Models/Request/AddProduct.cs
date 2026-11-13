namespace Product.Api.Features.Product.Models.Request;

public record AddProduct(string Title, string? Description, decimal Price, decimal DiscountPercentage);