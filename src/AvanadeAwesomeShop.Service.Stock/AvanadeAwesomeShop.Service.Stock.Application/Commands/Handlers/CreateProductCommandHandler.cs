using MediatR;
using AvanadeAwesomeShop.Service.Stock.Application.Commands;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;
using AvanadeAwesomeShop.Service.Stock.Domain.Entities;
using AvanadeAwesomeShop.Service.Stock.Domain.Repositories;

namespace AvanadeAwesomeShop.Service.Stock.Application.Commands.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            request.Product.Name,
            request.Product.Category,
            request.Product.Description,
            request.Product.Price,
            request.Product.Quantity
        );

        await _productRepository.AddAsync(product);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
