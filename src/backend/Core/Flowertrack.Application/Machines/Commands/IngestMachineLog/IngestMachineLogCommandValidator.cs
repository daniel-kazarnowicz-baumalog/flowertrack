using FluentValidation;

namespace Flowertrack.Application.Machines.Commands.IngestMachineLog;

/// <summary>
/// Validator for IngestMachineLogCommand
/// </summary>
public sealed class IngestMachineLogCommandValidator : AbstractValidator<IngestMachineLogCommand>
{
    private static readonly string[] ValidLogTypes = { "TELEMETRY", "STATUS", "ALARM", "WARNING", "INFO" };
    private static readonly string[] ValidSeverities = { "INFO", "WARNING", "ERROR", "CRITICAL" };
    private static readonly string[] ValidStatuses = { "NORMAL", "ALARM", "WARNING", "MAINTENANCE" };

    public IngestMachineLogCommandValidator()
    {
        RuleFor(x => x.MachineId)
            .NotEmpty()
            .WithMessage("Machine ID is required");

        RuleFor(x => x.LogContent)
            .NotEmpty()
            .WithMessage("Log content is required")
            .MaximumLength(65535)
            .WithMessage("Log content cannot exceed 65535 characters");

        RuleFor(x => x.LogType)
            .NotEmpty()
            .WithMessage("Log type is required")
            .Must(type => ValidLogTypes.Contains(type.ToUpperInvariant()))
            .WithMessage($"Log type must be one of: {string.Join(", ", ValidLogTypes)}");

        RuleFor(x => x.Severity)
            .NotEmpty()
            .WithMessage("Severity is required")
            .Must(severity => ValidSeverities.Contains(severity.ToUpperInvariant()))
            .WithMessage($"Severity must be one of: {string.Join(", ", ValidSeverities)}");

        RuleFor(x => x.Status)
            .Must(status => string.IsNullOrEmpty(status) || ValidStatuses.Contains(status.ToUpperInvariant()))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.AlarmCode)
            .MaximumLength(100)
            .WithMessage("Alarm code cannot exceed 100 characters");

        RuleFor(x => x.AlarmMessage)
            .MaximumLength(500)
            .WithMessage("Alarm message cannot exceed 500 characters");

        // If LogType is ALARM, AlarmCode should be provided
        RuleFor(x => x.AlarmCode)
            .NotEmpty()
            .When(x => x.LogType.Equals("ALARM", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Alarm code is required when log type is ALARM");
    }
}
