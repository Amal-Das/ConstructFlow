using FluentValidation;

namespace ConstructFlow.Application.Inventory.Commands.RecordStockTransaction;

public class RecordStockTransactionCommandValidator : AbstractValidator<RecordStockTransactionCommand>
{
    public RecordStockTransactionCommandValidator()
    {
        RuleFor(x => x.InventoryItemId).GreaterThan(0);

        RuleFor(x => x.TransactionType)
            .NotEmpty()
            .Must(t => t is "StockIn" or "StockOut" or "Adjustment")
            .WithMessage("TransactionType must be StockIn, StockOut, or Adjustment.");

        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0)
            .WithMessage("Quantity cannot be negative.");
    }
}