using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.CompleteMaintenance;

public sealed class CompleteMaintenanceCommandValidator : AbstractValidator<CompleteMaintenanceCommand>
{
    public CompleteMaintenanceCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.CompletedDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date));
    }
}
