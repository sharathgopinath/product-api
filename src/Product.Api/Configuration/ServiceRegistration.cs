using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;
using Product.Api.Authentication;
using Serilog;
using Product.Api.Features.Product.Services;
using Product.Data;

namespace Product.Api.Configuration;

/// <summary>
/// Registers all application services: Serilog, JSON options, OpenAPI, Basic auth, validators, product service and repository.
/// </summary>
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

        builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
        {
            // Describe Basic auth so Swagger UI shows an Authorize button
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[BasicAuthenticationOptions.SchemeName] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                Description = "HTTP Basic authentication"
            };
            document.Security ??= [];
            document.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BasicAuthenticationOptions.SchemeName, document)] = []
            });
            return Task.CompletedTask;
        }));

        builder.Services.Configure<BasicAuthenticationOptions>(
            builder.Configuration.GetSection(BasicAuthenticationOptions.SectionName));
        builder.Services.AddAuthentication(BasicAuthenticationOptions.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationOptions.SchemeName, null);
        builder.Services.AddAuthorization();

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(ServiceRegistration));
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddSingleton<ProductRepository>();
    }
}