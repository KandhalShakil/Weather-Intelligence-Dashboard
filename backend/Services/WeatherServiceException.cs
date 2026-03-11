namespace backend.Services;

public sealed class WeatherServiceException : Exception
{
    public int StatusCode { get; }

    public WeatherServiceException(int statusCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}