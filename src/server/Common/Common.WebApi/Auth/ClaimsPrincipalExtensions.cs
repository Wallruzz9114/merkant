using System.Security.Claims;

namespace Common.WebAPI.Auth;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentException(null, nameof(principal));

        Claim? claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value;
    }

    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentException(null, nameof(principal));

        Claim? claim = principal.FindFirst(ClaimTypes.Email);
        return claim?.Value;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        if (principal is null)
            throw new ArgumentException(null, nameof(principal));

        Claim? claim = principal.FindFirst(ClaimTypes.Name);
        return claim?.Value;
    }
}