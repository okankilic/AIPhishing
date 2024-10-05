using AIPhishing.Business.Auth.Models;
using AIPhishing.Business.Contexts;

namespace AIPhishing.Business.Auth;

public interface IAuthBusiness
{
    Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request);
    Task UpdatePasswordAsync(AuthUpdatePasswordRequest request, UserContext currentUser);
}