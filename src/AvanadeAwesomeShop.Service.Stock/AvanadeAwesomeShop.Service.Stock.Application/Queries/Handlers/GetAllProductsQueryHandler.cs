using MediatR;
using AvanadeAwesomeShop.Service.Stock.Application.Queries;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;
using AvanadeAwesomeShop.Service.Stock.Domain.Repositories;

namespace AvanadeAwesomeShop.Service.Stock.Application.Queries.Handlers;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        });
    }
}
