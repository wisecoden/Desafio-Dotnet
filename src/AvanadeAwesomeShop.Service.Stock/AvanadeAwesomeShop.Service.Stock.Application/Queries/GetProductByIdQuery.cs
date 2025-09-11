using MediatR;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Stock.Application.Queries;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto?>;
