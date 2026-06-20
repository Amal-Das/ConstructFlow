using ConstructFlow.Contracts.PurchaseRequests;
using MediatR;

namespace ConstructFlow.Application.PurchaseRequests.Queries.GetAllPurchaseRequests;

public class GetAllPurchaseRequestsQuery : IRequest<List<PurchaseRequestListDto>>
{
}