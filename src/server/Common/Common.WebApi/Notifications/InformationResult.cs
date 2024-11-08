namespace Common.WebApi.Notifications;

public class InformationResult(string message)
{
    public string Message { get; } = message;
}