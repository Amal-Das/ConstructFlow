using AutoMapper;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.Vendors;
using MediatR;

namespace ConstructFlow.Application.Vendors.Queries.GetAllVendors;

public class GetAllVendorsQueryHandler : IRequestHandler<GetAllVendorsQuery, List<VendorDto>>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public GetAllVendorsQueryHandler(IVendorRepository vendorRepository, IMapper mapper)
    {
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }

    public async Task<List<VendorDto>> Handle(GetAllVendorsQuery request, CancellationToken cancellationToken)
    {
        var vendors = await _vendorRepository.GetAllAsync();
        return _mapper.Map<List<VendorDto>>(vendors);
    }
}