using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Request = Product.Api.Features.Product.Models.Request;
using Response = Product.Api.Features.Product.Models.Response;

namespace Product.Api.Tests;

/// <summary>
/// Integration tests for the /product endpoints, hosted in-process with WebApplicationFactory.
/// </summary>
public class ProductEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = CreateAuthenticatedClient(factory);

    private static HttpClient CreateAuthenticatedClient(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("test_user:test_password"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        return client;
    }

    private static Request.AddProduct ValidAddProduct(int id) => new(
        Id: id,
        Title: "Test product",
        Description: "A product added by a test",
        Price: 99.99m,
        DiscountPercentage: 5m,
        Rating: 4.5m,
        Stock: 10,
        Brand: "TestBrand",
        Category: "testing",
        Thumbnail: "https://example.com/thumb.jpg",
        Images: ["https://example.com/1.jpg"]);

    private static Request.UpdateProduct ValidUpdateProduct(int id) => new(
        Id: id,
        Title: "iPhone 9",
        Description: "An apple mobile which is nothing like apple",
        Price: 549m,
        DiscountPercentage: 12.96m,
        Rating: 4.69m,
        Stock: 94,
        Brand: "Apple",
        Category: "smartphones",
        Thumbnail: "https://dummyjson.com/image/i/products/1/thumbnail.jpg",
        Images: ["https://dummyjson.com/image/i/products/1/1.jpg"]);

    private static async Task ShouldBeValidationProblem(HttpResponseMessage response, string field)
    {
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        problem.ShouldNotBeNull();
        problem.Errors.Keys.ShouldContain(field);
    }

    // GET /product/list

    [Fact]
    public async Task GetProducts_InvalidPageSize_Returns400()
    {
        var response = await _client.GetAsync("/product/list?pageSize=0", TestContext.Current.CancellationToken);

        await ShouldBeValidationProblem(response, "PageSize");
    }

    [Fact]
    public async Task GetProducts_ValidPaging_Returns200WithPage()
    {
        var response = await _client.GetAsync("/product/list?pageSize=5&pageNumber=1", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<Response.GetProducts>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Products.Count.ShouldBe(5);
        body.TotalCount.ShouldBeGreaterThan(0);
    }

    // GET /product?id=

    [Fact]
    public async Task GetProduct_InvalidId_Returns400()
    {
        var response = await _client.GetAsync("/product?id=0", TestContext.Current.CancellationToken);

        await ShouldBeValidationProblem(response, "Id");
    }

    [Fact]
    public async Task GetProduct_ExistingId_Returns200WithProduct()
    {
        var response = await _client.GetAsync("/product?id=1", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<Response.Product>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.Id.ShouldBe(1);
        body.Title.ShouldBe("iPhone 9");
    }

    // POST /product/add

    [Fact]
    public async Task AddProduct_InvalidBody_Returns400()
    {
        var request = ValidAddProduct(0) with { Title = "ab" };

        var response = await _client.PostAsJsonAsync("/product/add", request, TestContext.Current.CancellationToken);

        await ShouldBeValidationProblem(response, "Id");
    }

    [Fact]
    public async Task AddProduct_ValidBody_Returns200WithId()
    {
        const int newId = 9001;

        var response = await _client.PostAsJsonAsync("/product/add", ValidAddProduct(newId), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var id = await response.Content.ReadFromJsonAsync<int>(TestContext.Current.CancellationToken);
        id.ShouldBe(newId);
    }

    // POST /product/update

    [Fact]
    public async Task UpdateProduct_InvalidPrice_Returns400()
    {
        var request = ValidUpdateProduct(1) with { Price = 0 };

        var response = await _client.PostAsJsonAsync("/product/update", request, TestContext.Current.CancellationToken);

        await ShouldBeValidationProblem(response, "Price");
    }

    [Fact]
    public async Task UpdateProduct_ExistingId_Returns204()
    {
        var response = await _client.PostAsJsonAsync("/product/update", ValidUpdateProduct(1), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
