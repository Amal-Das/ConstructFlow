using ConstructFlow.Application.Common.Exceptions;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.PurchaseRequests;
using MediatR;

namespace ConstructFlow.Application.PurchaseRequests.Queries.GetPurchaseRequestById;

public class GetPurchaseRequestByIdQueryHandler : IRequestHandler<GetPurchaseRequestByIdQuery, PurchaseRequestListDto>
{
    private readonly IPurchaseRequestRepository _purchaseRequestRepository;

    public GetPurchaseRequestByIdQueryHandler(IPurchaseRequestRepository purchaseRequestRepository)
    {
        _purchaseRequestRepository = purchaseRequestRepository;
    }

    public async Task<PurchaseRequestListDto> Handle(GetPurchaseRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var all = await _purchaseRequestRepository.GetAllAsync();
        var found = all.FirstOrDefault(p => p.Id == request.Id);

        if (found is null)
            throw new NotFoundException("PurchaseRequest", request.Id);

        return found;
    }
}