using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using MediatR;

namespace ConstructFlow.Application.Vendors.Commands.CreateVendor;

public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommand, int>
{
    private readonly IVendorRepository _vendorRepository;

    public CreateVendorCommandHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<int> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = new Vendor
        {
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true
        };

        return await _vendorRepository.CreateAsync(vendor);
    }
}