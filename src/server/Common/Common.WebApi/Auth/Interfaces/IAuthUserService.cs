namespace Common.WebApi.Auth.Interfaces;

public interface IAuthUserService<TIdentityUser>
{
    Task<TIdentityUser?> GetUserByUsernameOrEmail(string usernameOrEmail);
    Task<int> GetFailedAccessAttempts(TIdentityUser user);
}