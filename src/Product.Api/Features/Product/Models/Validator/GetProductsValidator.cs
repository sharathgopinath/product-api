using FluentValidation;

namespace Product.Api.Features.Product.Models.Validator;

public class GetProductsValidator : AbstractValidator<Request.GetProducts>
{
    public GetProductsValidator()
    {
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);
    }
}
