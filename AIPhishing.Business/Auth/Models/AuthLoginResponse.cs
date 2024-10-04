namespace AIPhishing.Business.Auth.Models;

public record AuthLoginResponse(
    string ApiKey, 
    int TokenDurationInMinutes,
    DateTime TokenExpiry,
    AuthUserResponse User);