namespace Arkanis.Overlay.External.Medrunner;

public class MedrunnerApiException(
    string message,
    int statusCode,
    string? errorMessage = null,
    TimeSpan? retryAfter = null
) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string? ApiErrorMessage { get; } = errorMessage;
    public TimeSpan? RetryAfter { get; } = retryAfter;
}
