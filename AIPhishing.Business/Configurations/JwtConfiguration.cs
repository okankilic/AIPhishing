namespace AIPhishing.Business.Configurations;

public class JwtConfiguration
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 1440;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Secret))
        {
            throw new ArgumentNullException(nameof(Secret), "Secret should be set");
        }

        if (string.IsNullOrEmpty(Issuer))
        {
            throw new ArgumentNullException(nameof(Issuer), "Issuer should be set");
        }

        if (string.IsNullOrEmpty(Audience))
        {
            throw new ArgumentNullException(nameof(Audience), "Audience should be set");
        }

        if (ExpiresInMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ExpiresInMinutes), ExpiresInMinutes, "Must be higher than 0");
        }
    }
}