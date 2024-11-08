namespace Common.EventBus.Integrations;

public enum EventState
{
    NotPublished = 0,
    InProgress = 1,
    Published = 2,
    PublishedFailed = 3
}