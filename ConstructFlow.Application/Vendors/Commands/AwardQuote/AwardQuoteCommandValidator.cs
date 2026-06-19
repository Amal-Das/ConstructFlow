using FluentValidation;

namespace ConstructFlow.Application.Vendors.Commands.AwardQuote;

public class AwardQuoteCommandValidator : AbstractValidator<AwardQuoteCommand>
{
    public AwardQuoteCommandValidator()
    {
        RuleFor(x => x.PurchaseRequestId).GreaterThan(0);
        RuleFor(x => x.VendorQuoteId).GreaterThan(0);
    }
}