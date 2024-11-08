using Common.WebApi.Auth.Interfaces;
using Common.WebAPI.Auth;
using Microsoft.AspNetCore.Http;

namespace Common.WebApi.Auth.Services;

public class AuthService(IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetUserEmail() => _httpContextAccessor.HttpContext?.User.GetUserEmail() ?? "";

    public string GetUserId() => _httpContextAccessor.HttpContext?.User.GetUserId() ?? "";

    public string GetUserName() => _httpContextAccessor.HttpContext?.User.GetUserName() ?? "";

    public bool IsAuthenticated() => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}