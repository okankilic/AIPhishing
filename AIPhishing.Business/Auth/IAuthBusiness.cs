using AIPhishing.Business.Auth.Models;

namespace AIPhishing.Business.Auth;

public interface IAuthBusiness
{
    Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request);
}