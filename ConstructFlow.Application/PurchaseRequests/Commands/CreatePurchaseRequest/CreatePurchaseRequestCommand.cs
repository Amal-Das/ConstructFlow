using MediatR;

namespace ConstructFlow.Application.PurchaseRequests.Commands.CreatePurchaseRequest;

public class CreatePurchaseRequestCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public List<CreatePurchaseRequestItemDto> Items { get; set; } = new();
}

public class CreatePurchaseRequestItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Specification { get; set; }
}