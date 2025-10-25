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

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}
