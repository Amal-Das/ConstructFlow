using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Domain.Entities;
using ConstructFlow.Domain.Enums;
using MediatR;

namespace ConstructFlow.Application.PurchaseRequests.Commands.CreatePurchaseRequest;

public class CreatePurchaseRequestCommandHandler : IRequestHandler<CreatePurchaseRequestCommand, int>
{
    private readonly IPurchaseRequestRepository _purchaseRequestRepository;

    public CreatePurchaseRequestCommandHandler(IPurchaseRequestRepository purchaseRequestRepository)
    {
        _purchaseRequestRepository = purchaseRequestRepository;
    }

    public async Task<int> Handle(CreatePurchaseRequestCommand request, CancellationToken cancellationToken)
    {
        var requestNumber = $"PR-{DateTime.UtcNow:yyyyMMddHHmmss}";

        var purchaseRequest = new PurchaseRequest
        {
            ProjectId = request.ProjectId,
            RequestNumber = requestNumber,
            Status = PurchaseRequestStatus.Submitted,
            RequestedBy = request.RequestedBy,
            RequestDate = DateTime.UtcNow,
            Remarks = request.Remarks
        };

        var items = request.Items.Select(i => new PurchaseRequestItem
        {
            ItemName = i.ItemName,
            Unit = i.Unit,
            Quantity = i.Quantity,
            Specification = i.Specification
        }).ToList();

        return await _purchaseRequestRepository.CreateAsync(purchaseRequest, items);
    }
}