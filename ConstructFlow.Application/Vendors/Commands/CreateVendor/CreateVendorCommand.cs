using MediatR;

namespace ConstructFlow.Application.Vendors.Commands.CreateVendor;

public class CreateVendorCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
}