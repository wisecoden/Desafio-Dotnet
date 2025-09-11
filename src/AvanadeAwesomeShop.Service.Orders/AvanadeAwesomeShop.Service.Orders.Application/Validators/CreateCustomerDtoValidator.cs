using FluentValidation;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Orders.Application.Validators
{
    public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Nome completo é obrigatório")
                .MinimumLength(2)
                .WithMessage("Nome completo deve ter pelo menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("Nome completo não pode ter mais de 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email é obrigatório")
                .EmailAddress()
                .WithMessage("Email deve ter um formato válido")
                .MaximumLength(255)
                .WithMessage("Email não pode ter mais de 255 caracteres");
        }
    }
}
