using FluentValidation;

namespace Product.Api.Features.Product.Models.Validator;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeAbsoluteUrl<T>(this IRuleBuilder<T, string?> rule) =>
        rule.Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                         && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("'{PropertyName}' must be a valid http(s) URL.");
}
