using MediatR;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Stock.Application.Commands;

public record UpdateStockCommand(Guid ProductId, int Quantity) : IRequest<ProductDto>;
