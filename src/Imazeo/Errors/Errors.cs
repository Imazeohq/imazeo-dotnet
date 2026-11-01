namespace Imazeo.Errors;

public class ImazeoError : Exception
{
    public int? Status { get; }
    public string? Code { get; }
    public string? RequestId { get; }
    public object? Details { get; }

    public ImazeoError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Status = status;
        Code = code;
        RequestId = requestId;
        Details = details;
    }
}

public sealed class AuthenticationError : ImazeoError
{
    public AuthenticationError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public sealed class AuthorizationError : ImazeoError
{
    public AuthorizationError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public sealed class ValidationError : ImazeoError
{
    public ValidationError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public sealed class NotFoundError : ImazeoError
{
    public NotFoundError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public sealed class ConflictError : ImazeoError
{
    public ConflictError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public sealed class RateLimitError : ImazeoError
{
    public RateLimitError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public sealed class ApiError : ImazeoError
{
    public ApiError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public sealed class NetworkError : ImazeoError
{
    public NetworkError(
        string message,
        int? status = null,
        string? code = null,
        string? requestId = null,
        object? details = null,
        Exception? innerException = null)
        : base(message, status, code, requestId, details, innerException)
    {
    }
}

public static class ErrorMapper
{
    public static ImazeoError MapHttpError(
        int status,
        string message,
        string? code = null,
        string? requestId = null,
        object? details = null)
    {
        return status switch
        {
            401 => new AuthenticationError(message, status, code, requestId, details),
            403 => new AuthorizationError(message, status, code, requestId, details),
            404 => new NotFoundError(message, status, code, requestId, details),
            409 => new ConflictError(message, status, code, requestId, details),
            429 => new RateLimitError(message, status, code, requestId, details),
            400 or 413 or 415 => new ValidationError(message, status, code, requestId, details),
            _ => new ApiError(message, status, code, requestId, details),
        };
    }
}
