using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvanadeAwesomeShop.Service.Stock.Application.Commands;
using AvanadeAwesomeShop.Service.Stock.Application.Queries;
using AvanadeAwesomeShop.Service.Stock.Application.Dtos;


namespace AvanadeAwesomeShop.Service.Stock.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    // [Authorize(Roles = "Admin,Manager,User")] // Todos podem ver produtos
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    // [Authorize(Roles = "Admin,Manager,User")] // Todos podem ver produto específico
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin,Manager")] // Apenas Admin/Manager podem criar produtos
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var command = new CreateProductCommand(createProductDto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin,Manager")] // Apenas Admin/Manager podem atualizar estoque
    public async Task<ActionResult<ProductDto>> UpdateStock([FromRoute] Guid id, [FromBody] UpdateStockDto updateStockDto)
    {
        try
        {
            var command = new UpdateStockCommand(id, updateStockDto.Quantity);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
