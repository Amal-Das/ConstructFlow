using ConstructFlow.Application.Vendors.Commands.AwardQuote;
using ConstructFlow.Application.Vendors.Commands.CreateVendor;
using ConstructFlow.Application.Vendors.Commands.SubmitVendorQuote;
using ConstructFlow.Application.Vendors.Queries.GetAllVendors;
using ConstructFlow.Application.Vendors.Queries.GetQuoteComparison;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VendorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllVendorsQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpPost("quotes")]
    public async Task<IActionResult> SubmitQuote([FromBody] SubmitVendorQuoteCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpGet("comparison/{purchaseRequestId:int}")]
    public async Task<IActionResult> GetComparison(int purchaseRequestId)
    {
        var result = await _mediator.Send(new GetQuoteComparisonQuery(purchaseRequestId));
        return Ok(result);
    }

    [HttpPost("award")]
    public async Task<IActionResult> AwardQuote([FromBody] AwardQuoteCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}