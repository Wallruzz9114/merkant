using Common.WebApi.Utils.Interfaces;

namespace Common.WebApi.Results;

public record ResultUnauthorized : Result, IResultUnauthorized { }

public record ResultUnauthorized<T>() : Result<T>(), IResultUnauthorized { }