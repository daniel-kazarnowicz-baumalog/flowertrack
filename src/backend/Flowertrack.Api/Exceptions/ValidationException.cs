namespace Flowertrack.Api.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : BaseException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string field, string error)
        : base($"Validation failed for {field}: {error}")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}
