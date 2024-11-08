namespace Common.WebApi.Notifications;

public class WarningResult(string message)
{
    public string Message { get; } = message;
}