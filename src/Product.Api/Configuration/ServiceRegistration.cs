using FluentValidation;
using Product.Api.Features.Product.Services;

namespace Product.Api.Configuration;

public static class ServiceRegistration
{
    public static void RegisterAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(ServiceRegistration));
        builder.Services.AddScoped<ProductService>();
    }
}