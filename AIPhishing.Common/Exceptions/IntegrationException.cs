namespace AIPhishing.Common.Exceptions;

public class IntegrationException(string application, string message)
    : Exception(message)
{
    public string Application { get; init; } = application;
}