using FluentValidation;

namespace Product.Api.Features.Product.Models.Validator;

public class GetProductValidator : AbstractValidator<Request.GetProduct>
{
    public GetProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}
