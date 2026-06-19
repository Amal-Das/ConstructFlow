namespace ConstructFlow.Contracts.PurchaseRequests;

public class PurchaseRequestDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string RequestNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string? Remarks { get; set; }
    public List<PurchaseRequestItemDto> Items { get; set; } = new();
}