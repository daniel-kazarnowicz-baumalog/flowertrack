using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests using FluentValidation
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            foreach (var failure in failures)
            {
                _logger.LogWarning(
                    "Validation failed for {RequestType}: Property '{PropertyName}' - {ErrorMessage}",
                    typeof(TRequest).Name,
                    failure.PropertyName,
                    failure.ErrorMessage);
            }

            throw new Exceptions.ValidationException(failures);
        }

        return await next();
    }
}
