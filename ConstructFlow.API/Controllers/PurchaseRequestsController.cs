using ConstructFlow.Application.PurchaseRequests.Commands.CreatePurchaseRequest;
using ConstructFlow.Application.PurchaseRequests.Queries.GetAllPurchaseRequests;
using ConstructFlow.Application.PurchaseRequests.Queries.GetPurchaseRequestById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchaseRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllPurchaseRequestsQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetPurchaseRequestByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseRequestCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }
}