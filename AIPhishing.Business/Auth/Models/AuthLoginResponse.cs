namespace AIPhishing.Business.Auth.Models;

public record AuthLoginResponse(
    string ApiKey, 
    AuthUserResponse User);