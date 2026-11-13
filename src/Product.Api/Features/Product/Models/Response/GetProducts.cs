namespace Product.Api.Features.Product.Models.Response;

public class GetProducts
{
    public List<Product> Products { get; set; } = [];
    public int TotalCount { get; set; }
}
