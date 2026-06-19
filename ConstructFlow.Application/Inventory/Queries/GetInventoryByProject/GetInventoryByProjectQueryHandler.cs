using AutoMapper;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.Inventory;
using MediatR;

namespace ConstructFlow.Application.Inventory.Queries.GetInventoryByProject;

public class GetInventoryByProjectQueryHandler : IRequestHandler<GetInventoryByProjectQuery, List<InventoryItemDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public GetInventoryByProjectQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<List<InventoryItemDto>> Handle(GetInventoryByProjectQuery request, CancellationToken cancellationToken)
    {
        var items = await _inventoryRepository.GetByProjectIdAsync(request.ProjectId);
        return _mapper.Map<List<InventoryItemDto>>(items);
    }
}