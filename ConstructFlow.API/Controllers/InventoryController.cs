using ConstructFlow.Application.Inventory.Commands.CreateInventoryItem;
using ConstructFlow.Application.Inventory.Commands.RecordStockTransaction;
using ConstructFlow.Application.Inventory.Queries.GetInventoryByProject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _mediator.Send(new GetInventoryByProjectQuery(projectId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateItem([FromBody] CreateInventoryItemCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpPost("transaction")]
    public async Task<IActionResult> RecordTransaction([FromBody] RecordStockTransactionCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}