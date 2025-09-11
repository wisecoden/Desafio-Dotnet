using FluentValidation;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Stock.Application.Validators;

public class UpdateStockDtoValidator : AbstractValidator<UpdateStockDto>
{
    public UpdateStockDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEqual(0).WithMessage("Quantity cannot be zero");
    }
}
