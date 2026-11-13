namespace Product.Api.Features.Product.Models.Response;

public class GetProducts
{
    public List<Product> Products = [];
    public int TotalCount { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}