using Common.WebApi.Utils.Interfaces;

namespace Common.WebApi.Results;

public record ResultNotFound : Result, IResultNotFound
{
    public ResultNotFound() { }
}
public record ResultNotFound<T>() : Result<T>(), IResultNotFound { }