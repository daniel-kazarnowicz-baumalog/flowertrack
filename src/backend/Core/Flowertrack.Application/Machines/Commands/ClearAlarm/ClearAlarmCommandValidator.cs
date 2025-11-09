using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.ClearAlarm;

public sealed class ClearAlarmCommandValidator : AbstractValidator<ClearAlarmCommand>
{
    public ClearAlarmCommandValidator()
    {
        RuleFor(x => x.MachineId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
