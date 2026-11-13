using System.Text.Json;

namespace Product.Data;

/// <summary>
/// In-memory product store, hydrated from DataSource/test_products.json.
/// Must be registered as a singleton. Changes are not written back to the file.
/// Not thread-safe: intended for local/dev use only.
/// </summary>
public class ProductRepository
{
    private static readonly string DataFilePath =
        Path.Combine(AppContext.BaseDirectory, "DataSource", "test_products.json");

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private List<Models.Product> _products = [];

    /// <summary>Loads (or reloads) the product list from the JSON data file into memory.</summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(DataFilePath);
        var dataFile = await JsonSerializer.DeserializeAsync<Models.ProductDataFile>(stream, JsonOptions, cancellationToken)
                       ?? throw new InvalidOperationException($"Product data file '{DataFilePath}' is empty.");

        _products = dataFile.Products.OrderBy(p => p.Id).ToList();
    }

    /// <summary>Returns one page of products (1-based page number) and the total product count.</summary>
    public Task<(IReadOnlyList<Models.Product> Products, int TotalCount)> GetProducts(int pageSize, int pageNumber)
    {
        IReadOnlyList<Models.Product> page = _products
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((page, _products.Count));
    }

    /// <summary>Returns the product with the given id, or null if it doesn't exist.</summary>
    public Task<Models.Product?> GetProduct(int id) =>
        Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    /// <summary>
    /// Returns true if a product other than <paramref name="excludeId"/> has the same title and brand.
    /// Comparison is case-insensitive and ignores surrounding whitespace; a null brand equals an empty one.
    /// </summary>
    public Task<bool> ExistsByTitleAndBrand(string title, string? brand, int? excludeId = null)
    {
        var normalizedTitle = title.Trim();
        var normalizedBrand = (brand ?? string.Empty).Trim();

        var exists = _products.Any(p =>
            p.Id != excludeId
            && string.Equals(p.Title.Trim(), normalizedTitle, StringComparison.OrdinalIgnoreCase)
            && string.Equals((p.Brand ?? string.Empty).Trim(), normalizedBrand, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    /// <summary>Adds a product with the caller-supplied id. Returns false if the id is already taken.</summary>
    public Task<bool> AddProduct(Models.Product product)
    {
        if (_products.Any(p => p.Id == product.Id))
            return Task.FromResult(false);

        // Insert at the position that keeps the list sorted by id.
        var index = _products.FindIndex(p => p.Id > product.Id);
        _products.Insert(index < 0 ? _products.Count : index, product);
        return Task.FromResult(true);
    }

    /// <summary>Replaces the product with the same id. Returns false if no such product exists.</summary>
    public Task<bool> UpdateProduct(Models.Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index < 0)
            return Task.FromResult(false);

        _products[index] = product;
        return Task.FromResult(true);
    }
}
