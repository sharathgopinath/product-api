using FluentValidation;
using Product.Api.Features.Product;
using Product.Api.Middleware;
using Product.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add dependencies
builder.Services.AddOpenApi();
builder.RegisterAppServices();

var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
