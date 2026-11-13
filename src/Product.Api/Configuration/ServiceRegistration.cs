using System.Text.Json.Serialization;
using FluentValidation;
using Serilog;
using Product.Api.Features.Product.Services;
using Product.Data;

namespace Product.Api.Configuration;

public static class ServiceRegistration
{
    public static void RegisterAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        // Strict number handling keeps OpenAPI number schemas a single type ("integer")
        builder.Services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(ServiceRegistration));
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddSingleton<ProductRepository>();
    }
}