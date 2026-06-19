using ConstructFlow.Domain.Common;

namespace ConstructFlow.Domain.Entities;

public class Vendor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<VendorQuote> VendorQuotes { get; set; } = new List<VendorQuote>();
}