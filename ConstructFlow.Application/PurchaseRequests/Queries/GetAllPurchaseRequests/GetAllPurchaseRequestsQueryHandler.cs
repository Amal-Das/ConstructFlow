using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.PurchaseRequests;
using MediatR;

namespace ConstructFlow.Application.PurchaseRequests.Queries.GetAllPurchaseRequests;

public class GetAllPurchaseRequestsQueryHandler : IRequestHandler<GetAllPurchaseRequestsQuery, List<PurchaseRequestListDto>>
{
    private readonly IPurchaseRequestRepository _purchaseRequestRepository;

    public GetAllPurchaseRequestsQueryHandler(IPurchaseRequestRepository purchaseRequestRepository)
    {
        _purchaseRequestRepository = purchaseRequestRepository;
    }

    public async Task<List<PurchaseRequestListDto>> Handle(GetAllPurchaseRequestsQuery request, CancellationToken cancellationToken)
    {
        return await _purchaseRequestRepository.GetAllAsync();
    }
}