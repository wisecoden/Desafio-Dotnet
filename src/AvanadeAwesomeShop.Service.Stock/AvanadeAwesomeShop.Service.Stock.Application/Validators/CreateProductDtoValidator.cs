using FluentValidation;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Stock.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Product category is required")
            .MaximumLength(50).WithMessage("Product category must not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Product description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to zero");
    }
}
