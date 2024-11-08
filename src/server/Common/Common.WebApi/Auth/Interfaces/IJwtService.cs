namespace Common.WebApi.Auth.Interfaces;

public interface IJwtService
{
    Task<AccessTokenDto> GenerateToken(string username);
    Task<string> GenerateRefreshToken(string username);
    Task<(bool, string)> ValidateRefreshToken(string refreshToken);
}