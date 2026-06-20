using System.Data;
using ConstructFlow.Application.Common.Interfaces;
using ConstructFlow.Contracts.Vendors;
using ConstructFlow.Domain.Entities;
using Dapper;

namespace ConstructFlow.Infrastructure.Persistence.Repositories;

public class VendorQuoteRepository : IVendorQuoteRepository
{
    private readonly DapperContext _context;

    public VendorQuoteRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> SubmitQuoteAsync(VendorQuote quote, List<VendorQuoteItem> items)
    {
        var itemsTable = new DataTable();
        itemsTable.Columns.Add("PurchaseRequestItemId", typeof(int));
        itemsTable.Columns.Add("UnitPrice", typeof(decimal));

        foreach (var item in items)
        {
            itemsTable.Rows.Add(item.PurchaseRequestItemId, item.UnitPrice);
        }

        var parameters = new DynamicParameters();
        parameters.Add("PurchaseRequestId", quote.PurchaseRequestId, DbType.Int32);
        parameters.Add("VendorId", quote.VendorId, DbType.Int32);
        parameters.Add("QuoteDate", quote.QuoteDate, DbType.DateTime);
        parameters.Add("Remarks", quote.Remarks, DbType.String);
        parameters.Add("Items", itemsTable.AsTableValuedParameter("VND.tvp_VendorQuoteItem"));
        parameters.Add("NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "VND.usp_SubmitVendorQuote",
            parameters,
            commandType: CommandType.StoredProcedure);

        var returnStatus = parameters.Get<string>("ReturnStatus");
        if (returnStatus != "SUCCESS")
        {
            var errorCode = parameters.Get<string>("ErrorCode") ?? "UNKNOWN_ERROR";
            throw new ConstructFlow.Application.Common.Exceptions.BusinessRuleException(
                errorCode, "Failed to submit vendor quote. Please check the entered prices and try again.");
        }

        return parameters.Get<int>("NewId");
    }

    public async Task<QuoteComparisonDto> GetComparisonDataAsync(int purchaseRequestId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("PurchaseRequestId", purchaseRequestId, DbType.Int32);

        using var connection = _context.CreateConnection();
        using var multi = await connection.QueryMultipleAsync(
            "VND.usp_GetQuoteComparisonData",
            parameters,
            commandType: CommandType.StoredProcedure);

        var prItems = (await multi.ReadAsync<PrItemFlat>()).ToList();
        var vendorSummaries = (await multi.ReadAsync<VendorSummaryFlat>()).ToList();
        var priceCells = (await multi.ReadAsync<PriceCellFlat>()).ToList();

        var result = new QuoteComparisonDto
        {
            PurchaseRequestId = purchaseRequestId,
            VendorSummaries = vendorSummaries.Select(v => new VendorQuoteSummary
            {
                VendorId = v.VendorId,
                VendorName = v.VendorName,
                TotalAmount = v.TotalAmount,
                VendorQuoteId = v.VendorQuoteId,
                IsAwarded = v.IsAwarded
            }).ToList(),
            ItemRows = prItems.Select(item => new QuoteComparisonItemRow
            {
                PurchaseRequestItemId = item.PurchaseRequestItemId,
                ItemName = item.ItemName,
                Quantity = item.Quantity,
                VendorPrices = priceCells
                    .Where(p => p.PurchaseRequestItemId == item.PurchaseRequestItemId)
                    .Select(p => new VendorPriceCell
                    {
                        VendorId = p.VendorId,
                        UnitPrice = p.UnitPrice,
                        TotalPrice = p.TotalPrice
                    }).ToList()
            }).ToList()
        };

        return result;
    }

    public async Task AwardQuoteAsync(int purchaseRequestId, int vendorQuoteId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("PurchaseRequestId", purchaseRequestId, DbType.Int32);
        parameters.Add("VendorQuoteId", vendorQuoteId, DbType.Int32);
        parameters.Add("ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(
            "VND.usp_AwardQuote",
            parameters,
            commandType: CommandType.StoredProcedure);

        var returnStatus = parameters.Get<string>("ReturnStatus");
        if (returnStatus != "SUCCESS")
        {
            var errorCode = parameters.Get<string>("ErrorCode") ?? "UNKNOWN_ERROR";
            var message = errorCode == "QUOTE_NOT_FOUND"
                ? "The selected quote could not be found for this purchase request."
                : "Failed to award the quote.";

            throw new ConstructFlow.Application.Common.Exceptions.BusinessRuleException(errorCode, message);
        }
    }

    // Flat DTOs matching each result set exactly
    private class PrItemFlat
    {
        public int PurchaseRequestItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }

    private class VendorSummaryFlat
    {
        public int VendorQuoteId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsAwarded { get; set; }
    }

    private class PriceCellFlat
    {
        public int PurchaseRequestItemId { get; set; }
        public int VendorId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}