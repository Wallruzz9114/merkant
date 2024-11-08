using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.WebApi.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Common.WebApi.Auth.Services;

public class JwtService<TIdentityUser, TKey> : IJwtService where TIdentityUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
{
    private const string LAST_REFRESH_TOKEN = "__LastRefreshToken";

    private readonly UserManager<TIdentityUser> _userManager;
    private readonly IOptions<AuthSettings> _authSettings;
    private readonly IAuthUserService<TIdentityUser> _userAuthService;

    public JwtService(UserManager<TIdentityUser> userManager, IOptions<AuthSettings> settings, IAuthUserService<TIdentityUser> authUserService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _authSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        _userAuthService = authUserService ?? throw new ArgumentNullException(nameof(authUserService));
    }

    public async Task<string> GenerateRefreshToken(string username)
    {
        string jti = Guid.NewGuid().ToString();
        SigningCredentials key = GetCurrentKey();
        TIdentityUser? user = await _userAuthService.GetUserByUsernameOrEmail(username);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user!.Id.ToString()!),
            new(JwtRegisteredClaimNames.Jti, jti)
        ];

        ClaimsIdentity identityClaims = new();
        identityClaims.AddClaims(claims);

        JwtSecurityTokenHandler handler = new();

        SecurityToken securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _authSettings.Value.Issuer,
            Audience = _authSettings.Value.Audience,
            SigningCredentials = key,
            Subject = identityClaims,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_authSettings.Value.RefreshTokenExpiration),
            TokenType = "refreshtoken"
        });

        await UpdateLastRefreshToken(jti, user);

        return handler.WriteToken(securityToken);
    }

    public async Task<AccessTokenDto> GenerateToken(string username)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        SigningCredentials key = GetCurrentKey();
        TIdentityUser? user = await _userAuthService.GetUserByUsernameOrEmail(username);
        ClaimsIdentity? identityClaims = new();

        identityClaims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()!));
        identityClaims.AddClaim(new Claim(ClaimTypes.Email, user.Email!));
        identityClaims.AddClaim(new Claim(ClaimTypes.Name, user.UserName!));
        identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        userRoles.ToList().ForEach(x => identityClaims.AddClaim(new Claim(ClaimTypes.Role, x)));

        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        JwtService<TIdentityUser, TKey>.RemoveRefreshToken(userClaims);
        identityClaims.AddClaims(userClaims);

        SecurityToken token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _authSettings.Value.Issuer,
            Audience = _authSettings.Value.Audience,
            Subject = identityClaims,
            Expires = DateTime.UtcNow.AddMinutes(_authSettings.Value.ExpiresIn),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = key,
            TokenType = "jwt"
        });

        return new AccessTokenDto(
            accessToken: tokenHandler.WriteToken(token),
            expiresIn: _authSettings.Value.ExpiresIn,
            tokenType: JwtBearerDefaults.AuthenticationScheme,
            refreshToken: await GenerateRefreshToken(username)
        );
    }

    public async Task<(bool, string)> ValidateRefreshToken(string refreshToken)
    {
        JsonWebTokenHandler handler = new();

        TokenValidationResult result = await handler.ValidateTokenAsync(refreshToken, new TokenValidationParameters()
        {
            RequireSignedTokens = false,
            ValidIssuer = _authSettings.Value.Issuer,
            ValidAudience = _authSettings.Value.Audience,
            IssuerSigningKey = GetCurrentKey().Key,
        });

        if (!result.IsValid)
            throw new SecurityTokenException("Refresh token invalid");

        TIdentityUser? user = await _userManager.FindByIdAsync(JwtService<TIdentityUser, TKey>.GetUserId(result.ClaimsIdentity)!);

        if (user is null)
            throw new SecurityTokenException("User not found");

        IList<Claim>? claims = await _userManager.GetClaimsAsync(user);
        string? jti = GetJwtId(result.ClaimsIdentity);

        if (!claims.Any(c => c.Type == LAST_REFRESH_TOKEN && c.Value == jti))
            throw new SecurityTokenException("Refresh token already used");

        if (user!.LockoutEnabled)
        {
            if (user.LockoutEnd < DateTimeOffset.UtcNow)
                throw new SecurityTokenException("Usuário bloqueado");
        }

        return (true, user.UserName)!;
    }

    private static void RemoveRefreshToken(ICollection<Claim> claims)
    {
        Claim? refreshToken = claims.FirstOrDefault(f => f.Type == LAST_REFRESH_TOKEN);

        if (refreshToken is not null)
            claims.Remove(refreshToken);
    }

    private async Task UpdateLastRefreshToken(string jti, TIdentityUser user)
    {
        IList<Claim> claims = await _userManager.GetClaimsAsync(user);
        Claim newLastRtClaim = new(LAST_REFRESH_TOKEN, jti);

        Claim? claimLastRt = claims.FirstOrDefault(f => f.Type == LAST_REFRESH_TOKEN);
        if (claimLastRt is not null)
            await _userManager.ReplaceClaimAsync(user, claimLastRt, newLastRtClaim);
        else
            await _userManager.AddClaimAsync(user, newLastRtClaim);
    }

    private static string? GetJwtId(ClaimsIdentity principal) => principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

    private static string? GetUserId(ClaimsIdentity principal)
    {
        if (principal is null)
            throw new ArgumentException(null, nameof(principal));

        Claim? claim = principal.FindFirst(JwtRegisteredClaimNames.Sub) ?? principal.FindFirst(ClaimTypes.NameIdentifier);

        return claim?.Value;
    }

    private SigningCredentials GetCurrentKey() =>
        new(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authSettings.Value.SecretKey)), SecurityAlgorithms.HmacSha256Signature);
}