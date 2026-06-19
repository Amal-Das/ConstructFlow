using ConstructFlow.Contracts.Vendors;
using MediatR;

namespace ConstructFlow.Application.Vendors.Queries.GetAllVendors;

public class GetAllVendorsQuery : IRequest<List<VendorDto>>
{
}