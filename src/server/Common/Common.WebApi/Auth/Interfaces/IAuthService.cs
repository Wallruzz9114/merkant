namespace Common.WebApi.Auth.Interfaces;

public interface IAuthService
{
    string GetUserId();
    string GetUserEmail();
    string GetUserName();
    bool IsAuthenticated();
}