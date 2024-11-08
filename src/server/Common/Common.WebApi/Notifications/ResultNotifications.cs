using System.Text.Json.Serialization;
using Common.WebApi.Results;

namespace Common.WebApi.Notifications;

public record ResultNotifications
{
    public static ResultNotifications<TDataResponse> Ok<TDataResponse>(TDataResponse data)
        => new(data);

    public static ResultNotifications<TDataResponse> Fail<TDataResponse>(string message)
        => new(message);

    [JsonIgnore]
    public bool HasErrors => Errors.Any();
    public bool HasWarnings => Warnings.Any();
    public bool HasInformations => Informations.Any();

    [JsonIgnore]
    public List<ErrorResult> Errors { get; private set; } = new List<ErrorResult>();
    public List<WarningResult> Warnings { get; private set; } = new List<WarningResult>();
    public List<InformationResult> Informations { get; private set; } = new List<InformationResult>();

    public bool IsValid => !HasErrors;
}

public record ResultNotifications<TDataResponse> : ResultNotifications
{
    public TDataResponse? Data { get; private set; }

    public ResultNotifications(TDataResponse data)
    {
        Data = data;
    }

    public ResultNotifications(string message)
    {
        Errors.Add(new ErrorResult(null, null, message));
    }
}