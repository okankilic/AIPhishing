namespace AIPhishing.Business.Configurations;

public class EmailConfiguration
{
    public string From { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public int MaxTryCount { get; set; } = 3;
    public int MaxFetchCount { get; set; } = 100;

    public void Validate()
    {
        if (string.IsNullOrEmpty(From))
            throw new ArgumentNullException(nameof(From));

        if (string.IsNullOrEmpty(Password))
            throw new ArgumentNullException(nameof(Password));

        if (string.IsNullOrEmpty(Host))
            throw new ArgumentNullException(nameof(Host));

        if (Port <= 0)
            throw new ArgumentOutOfRangeException(nameof(Port));
    }
}