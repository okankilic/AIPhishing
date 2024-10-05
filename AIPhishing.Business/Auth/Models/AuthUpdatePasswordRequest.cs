namespace AIPhishing.Business.Auth.Models;

public record AuthUpdatePasswordRequest(string OldPassword, string NewPassword);