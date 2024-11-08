using Common.WebApi.Utils.Interfaces;

namespace Common.WebApi.Results;

public record ResultForbidden : Result, IResultForbidden { }

public record ResultForbidden<T>() : Result<T>(), IResultForbidden { }