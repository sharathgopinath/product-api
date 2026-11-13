using FluentValidation;

namespace Product.Api.Features.Product.Models.Validator;

public class UpdateProductValidator : AbstractValidator<Request.UpdateProduct>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Description)
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThan(100_000_000);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100);
    }
}
