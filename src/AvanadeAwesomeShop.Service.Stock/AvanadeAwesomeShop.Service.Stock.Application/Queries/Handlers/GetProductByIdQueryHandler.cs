using MediatR;
using AvanadeAwesomeShop.Service.Stock.Application.Queries;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;
using AvanadeAwesomeShop.Service.Stock.Domain.Repositories;

namespace AvanadeAwesomeShop.Service.Stock.Application.Queries.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        
        if (product == null)
            return null;

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
