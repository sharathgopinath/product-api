using FluentValidation;

namespace Product.Api.Features.Product.Models.Validator;

public class UpdateProductValidator : AbstractValidator<Request.UpdateProduct>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(x => x.Description)
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThan(100_000_000);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100);

        // Optional fields: validated only when provided.
        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 10);

        RuleFor(x => x.Stock)
            .InclusiveBetween(0, 100_000);

        RuleFor(x => x.Brand)
            .MaximumLength(500);

        RuleFor(x => x.Category)
            .MaximumLength(500);

        RuleFor(x => x.Thumbnail)
            .MustBeAbsoluteUrl()
            .When(x => x.Thumbnail is not null);

        RuleForEach(x => x.Images)
            .MustBeAbsoluteUrl();
    }
}
