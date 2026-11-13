using FluentValidation;
using Serilog;
using Product.Api.Features.Product.Services;

namespace Product.Api.Configuration;

public static class ServiceRegistration
{
    public static void RegisterAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(ServiceRegistration));
        builder.Services.AddScoped<ProductService>();
    }
}