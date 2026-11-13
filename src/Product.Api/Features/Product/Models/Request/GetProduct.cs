using Microsoft.AspNetCore.Mvc;

namespace Product.Api.Features.Product.Models.Request;

public record GetProduct([FromQuery(Name = "id")] int Id);
