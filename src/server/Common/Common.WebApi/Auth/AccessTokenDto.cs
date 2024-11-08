namespace Common.WebApi.Auth;

public class AccessTokenDto(string accessToken, int expiresIn, string tokenType, string refreshToken)
{
    public string AccessToken { get; private set; } = accessToken;
    public int ExpiresIn { get; private set; } = expiresIn;
    public string TokenType { get; private set; } = tokenType;
    public string RefreshToken { get; private set; } = refreshToken;
    public long IssuedUtc { get; private set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}