using FluentValidation;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Orders.Application.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("Pedido deve ter pelo menos um item")
                .Must(items => items.Count <= 50)
                .WithMessage("Pedido não pode ter mais de 50 itens");

            RuleForEach(x => x.Items)
                .SetValidator(new CreateOrderItemDtoValidator());
        }
    }

    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantidade deve ser maior que zero")
                .LessThanOrEqualTo(1000)
                .WithMessage("Quantidade não pode ser maior que 1000");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Preço unitário deve ser maior que zero")
                .LessThanOrEqualTo(100000)
                .WithMessage("Preço unitário não pode ser maior que R$ 100.000");
        }
    }
}
