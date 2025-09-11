using MediatR;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Stock.Application.Commands;

public record CreateProductCommand(CreateProductDto Product) : IRequest<ProductDto>;
