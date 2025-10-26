namespace Flowertrack.Api.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class DomainException : BaseException
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
