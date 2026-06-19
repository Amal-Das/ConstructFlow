using ConstructFlow.Domain.Enums;
using FluentValidation;

namespace ConstructFlow.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid project id.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(300);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(BeAValidStatus).WithMessage("Invalid project status.");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero.");
    }

    private bool BeAValidStatus(string status)
    {
        return Enum.TryParse<ProjectStatus>(status, true, out _);
    }
}