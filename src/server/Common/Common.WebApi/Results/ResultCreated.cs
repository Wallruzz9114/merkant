using Common.WebApi.Utils.Interfaces;

namespace Common.WebApi.Results;

public record ResultCreated<T> : Result<T>, IResultCreated
{
    public string Location { get; private set; }

    public ResultCreated(string location, T data) : base(data)
    {
        Location = location;
    }
}