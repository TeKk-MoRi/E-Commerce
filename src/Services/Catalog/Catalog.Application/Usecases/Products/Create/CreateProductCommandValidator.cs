using Catalog.Application.Usecases.Products;
using FluentValidation;

namespace Catalog.Application.Usecases.Products.Create;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
