using MediatR;
using AvanadeAwesomeShop.Service.Orders.Application.Queries;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;
using AvanadeAwesomeShop.Service.Orders.Domain.Enums;

namespace AvanadeAwesomeShop.Service.Orders.Application.Queries.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithItemsByIdAsync(request.OrderId);
            
            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                StatusDescription = GetStatusDescription(order.Status),
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                }).ToList()
            };
        }

        private static string GetStatusDescription(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Started => "Iniciado",
                OrderStatus.Completed => "Completado",
                OrderStatus.Rejected => "Rejeitado",
                _ => "Desconhecido"
            };
        }
    }

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                StatusDescription = GetStatusDescription(order.Status),
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                }).ToList()
            });
        }

        private static string GetStatusDescription(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Started => "Iniciado",
                OrderStatus.Completed => "Completado",
                OrderStatus.Rejected => "Rejeitado",
                _ => "Desconhecido"
            };
        }
    }

    public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId);

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                StatusDescription = GetStatusDescription(order.Status),
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                }).ToList()
            });
        }

        private static string GetStatusDescription(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Started => "Iniciado",
                OrderStatus.Completed => "Completado",
                OrderStatus.Rejected => "Rejeitado",
                _ => "Desconhecido"
            };
        }
    }

    public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByStatusQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByStatusAsync(request.Status);

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                StatusDescription = GetStatusDescription(order.Status),
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                }).ToList()
            });
        }

        private static string GetStatusDescription(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Started => "Iniciado",
                OrderStatus.Completed => "Completado",
                OrderStatus.Rejected => "Rejeitado",
                _ => "Desconhecido"
            };
        }
    }

    // Customer Query Handlers
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            
            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                CreatedAt = customer.CreatedAt
            };
        }
    }

    public class GetCustomerByEmailQueryHandler : IRequestHandler<GetCustomerByEmailQuery, CustomerDto?>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByEmailQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto?> Handle(GetCustomerByEmailQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByEmailAsync(request.Email);
            
            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                CreatedAt = customer.CreatedAt
            };
        }
    }

    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllAsync();
            
            return customers.Select(customer => new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                CreatedAt = customer.CreatedAt
            });
        }
    }
}
