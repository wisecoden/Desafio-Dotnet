using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using AvanadeAwesomeShop.Service.Orders.Application.Commands.Handlers;
using AvanadeAwesomeShop.Service.Orders.Application.Validators;

namespace AvanadeAwesomeShop.Service.Orders.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // MediatR - CQRS
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));

            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(CreateOrderDtoValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateCustomerDtoValidator).Assembly); 

            return services;
        }
    }
}
