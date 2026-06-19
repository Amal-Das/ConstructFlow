using FluentValidation;

namespace ConstructFlow.Application.Vendors.Commands.SubmitVendorQuote;

public class SubmitVendorQuoteCommandValidator : AbstractValidator<SubmitVendorQuoteCommand>
{
    public SubmitVendorQuoteCommandValidator()
    {
        RuleFor(x => x.PurchaseRequestId)
            .GreaterThan(0).WithMessage("Valid purchase request is required.");

        RuleFor(x => x.VendorId)
            .GreaterThan(0).WithMessage("Valid vendor is required.");

        RuleFor(x => x.QuoteDate)
            .NotEmpty().WithMessage("Quote date is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one priced item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.PurchaseRequestItemId)
                .GreaterThan(0).WithMessage("Invalid item reference.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
        });
    }
}