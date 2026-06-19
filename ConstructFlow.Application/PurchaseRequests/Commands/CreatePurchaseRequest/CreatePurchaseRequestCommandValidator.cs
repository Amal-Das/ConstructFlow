using FluentValidation;

namespace ConstructFlow.Application.PurchaseRequests.Commands.CreatePurchaseRequest;

public class CreatePurchaseRequestCommandValidator : AbstractValidator<CreatePurchaseRequestCommand>
{
    public CreatePurchaseRequestCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Valid project is required.");

        RuleFor(x => x.RequestedBy)
            .NotEmpty().WithMessage("Requested by is required.")
            .MaximumLength(200);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemName)
                .NotEmpty().WithMessage("Item name is required.")
                .MaximumLength(200);

            item.RuleFor(i => i.Unit)
                .NotEmpty().WithMessage("Unit is required.")
                .MaximumLength(50);

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}