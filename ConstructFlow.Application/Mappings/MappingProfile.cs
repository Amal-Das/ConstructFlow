using AutoMapper;
using ConstructFlow.Contracts.Inventory;
using ConstructFlow.Contracts.Projects;
using ConstructFlow.Contracts.PurchaseRequests;
using ConstructFlow.Contracts.Vendors;
using ConstructFlow.Domain.Entities;

namespace ConstructFlow.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<PurchaseRequest, PurchaseRequestDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ProjectName, opt => opt.MapFrom(s => s.Project.Name));

        CreateMap<PurchaseRequestItem, PurchaseRequestItemDto>();

        CreateMap<Vendor, VendorDto>();

        CreateMap<InventoryItem, InventoryItemDto>()
            .ForMember(d => d.IsLowStock, opt => opt.MapFrom(s => s.QuantityInStock <= s.MinimumStockLevel));
    }
}