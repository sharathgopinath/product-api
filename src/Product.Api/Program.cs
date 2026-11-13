using FluentValidation;
using Product.Api.Features.Product;
using Product.Api.Middleware;
using Product.Api.Configuration;
using Product.Data;

var builder = WebApplication.CreateBuilder(args);

// Add dependencies
builder.RegisterAppServices();

var app = builder.Build();

// Hydrate the in-memory product store from the JSON data file
await app.Services.GetRequiredService<ProductRepository>().LoadAsync();

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Product API v1");
    });
}

app.UseHttpsRedirection();
app.MapProductEndpoints();

app.Run();
