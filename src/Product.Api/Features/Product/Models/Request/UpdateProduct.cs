namespace Product.Api.Features.Product.Models.Request;

public record UpdateProduct(string Title, string Description, decimal Price, decimal DiscountPercentage);