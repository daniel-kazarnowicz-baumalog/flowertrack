using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.ScheduleMaintenance;

public sealed class ScheduleMaintenanceCommandValidator : AbstractValidator<ScheduleMaintenanceCommand>
{
    public ScheduleMaintenanceCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.ScheduledDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date));
        When(x => x.IntervalDays.HasValue, () =>
        {
            RuleFor(x => x.IntervalDays).GreaterThan(0);
        });
    }
}
