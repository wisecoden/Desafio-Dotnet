using MediatR;
using AvanadeAwesomeShop.Service.Orders.Application.Commands;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;

namespace AvanadeAwesomeShop.Service.Orders.Application.Commands.Handlers
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            // Verificar se já existe um customer com este email
            var existingCustomer = await _customerRepository.GetByEmailAsync(request.CustomerDto.Email);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException($"Customer with email {request.CustomerDto.Email} already exists");
            }

            // Criar novo customer
            var customer = new Customer(request.CustomerDto.FullName, request.CustomerDto.Email);
            var createdCustomer = await _customerRepository.CreateAsync(customer);

            return new CustomerDto
            {
                Id = createdCustomer.Id,
                FullName = createdCustomer.FullName,
                Email = createdCustomer.Email,
                CreatedAt = createdCustomer.CreatedAt
            };
        }
    }
}
