using MediatR;
using AvanadeAwesomeShop.Service.Stock.Application.Commands;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;
using AvanadeAwesomeShop.Service.Stock.Domain.Repositories;

namespace AvanadeAwesomeShop.Service.Stock.Application.Commands.Handlers;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public UpdateStockCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
            throw new Exception($"Product with ID {request.ProductId} not found");

        product.UpdateStock(request.Quantity);
        await _productRepository.UpdateAsync(product);

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
