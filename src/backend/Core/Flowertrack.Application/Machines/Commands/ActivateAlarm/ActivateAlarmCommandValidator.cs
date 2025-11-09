using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.ActivateAlarm;

public sealed class ActivateAlarmCommandValidator : AbstractValidator<ActivateAlarmCommand>
{
    public ActivateAlarmCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
