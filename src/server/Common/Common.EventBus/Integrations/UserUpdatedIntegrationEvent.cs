namespace Common.EventBus.Integrations;

public record UserUpdatedIntegrationEvent : IntegrationEvent
{
    public string? UserId { get; private init; }
    public string? Nome { get; private init; }
    public string? Username { get; private init; }
    public string? Email { get; private init; }
    public string? AvatarUrl { get; private init; }

    public UserUpdatedIntegrationEvent(string userId, string nome, string username, string email, string? avatarUrl)
    {
        UserId = userId;
        Nome = nome;
        Username = username;
        Email = email;
        AvatarUrl = avatarUrl;
    }

    public UserUpdatedIntegrationEvent(string userId, string nome, string username, string email, string? avatarUrl, string id, DateTimeOffset creationDate, string? key) : base(id, creationDate, key)
    {
        UserId = userId;
        Nome = nome;
        Username = username;
        Email = email;
        AvatarUrl = avatarUrl;
    }
}