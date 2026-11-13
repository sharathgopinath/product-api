using Microsoft.AspNetCore.Mvc;

namespace Product.Api.Features.Product.Models.Request;

public record GetProducts(
    [FromQuery(Name = "pageSize")] int PageSize = 10,
    [FromQuery(Name = "pageNumber")] int PageNumber = 1);
