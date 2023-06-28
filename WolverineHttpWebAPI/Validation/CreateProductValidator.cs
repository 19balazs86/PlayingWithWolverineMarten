using FluentValidation;
using WolverineHttpWebAPI.Entities;

namespace WolverineHttpWebAPI.Validation;

public sealed class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        RuleFor(cp => cp.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(50);

        RuleFor(cp => cp.Price)
            .GreaterThan(0);

        RuleFor(cp => cp.Description)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(250);
    }
}
