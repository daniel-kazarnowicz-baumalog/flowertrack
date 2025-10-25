namespace Flowertrack.Api.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found")
    {
    }
}
