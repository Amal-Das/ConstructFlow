using ConstructFlow.Contracts.PurchaseRequests;
using MediatR;

namespace ConstructFlow.Application.PurchaseRequests.Queries.GetPurchaseRequestById;

public class GetPurchaseRequestByIdQuery : IRequest<PurchaseRequestListDto>
{
    public int Id { get; set; }

    public GetPurchaseRequestByIdQuery(int id)
    {
        Id = id;
    }
}