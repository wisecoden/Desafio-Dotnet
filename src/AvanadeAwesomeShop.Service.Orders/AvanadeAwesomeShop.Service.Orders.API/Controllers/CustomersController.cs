using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvanadeAwesomeShop.Service.Orders.Application.Commands;
using AvanadeAwesomeShop.Service.Orders.Application.Queries;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Orders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    // [Authorize(Roles = "Admin,Manager")] // Todos podem ver clientes
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        var query = new GetAllCustomersQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    // [Authorize(Roles = "Admin,Manager,User")] // Todos podem ver cliente específico
    public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpGet("by-email/{email}")]
    // [Authorize(Roles = "Admin,Manager,User")] // Todos podem buscar cliente por email
    public async Task<ActionResult<CustomerDto>> GetCustomerByEmail(string email)
    {
        var query = new GetCustomerByEmailQuery(email);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    // [Authorize(Roles = "Admin,Manager")] // Apenas Admin/Manager podem criar clientes
  public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
  {
    try
    {
      var command = new CreateCustomerCommand(createCustomerDto);
      var result = await _mediator.Send(command);
      return CreatedAtAction(nameof(GetCustomer), new { id = result.Id }, result);
    }
    catch (InvalidOperationException ex)
    {
      return Conflict(ex.Message);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }
}
