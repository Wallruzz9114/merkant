namespace Common.WebApi.Results;

public record Result
{
    public static Result<TDataResponse> Ok<TDataResponse>(TDataResponse data) =>
        new(data);

    public static Result<TDataResponse> Created<TDataResponse>(string location, TDataResponse data) =>
        new ResultCreated<TDataResponse>(location, data);

    public static Result<TDataResponse> Fail<TDataResponse>(string message) =>
        new(message);

    public static Result<TDataResponse> Fail<TDataResponse>(string property, string message) =>
        new(property, message);

    public static Result<TDataResponse> Fail<TDataResponse>(string? code, string? property, string message) =>
        new(code, property, message);

    public static Result NotFound() => new ResultNotFound();
    public static Result<TDataResponse> NotFound<TDataResponse>() => new ResultNotFound<TDataResponse>();

    public static Result Unauthorized() => new ResultUnauthorized();
    public static Result<TDataResponse> Unauthorized<TDataResponse>() => new ResultUnauthorized<TDataResponse>();

    public static Result Forbidden() => new ResultForbidden();
    public static Result<TDataResponse> Forbidden<TDataResponse>() => new ResultForbidden<TDataResponse>();

    public static Result Fail(string message) => new(message);

    public static Result Fail(string property, string message) => new(property, message);

    public static Result Fail(string? code, string? property, string message) => new(code, property, message);

    public static Result Fail(IList<ErrorResult> errors) => new(errors);

    public static Result Ok() => new ResultNoContent();

    public static Result<TDataResponse> Fail<TDataResponse>(ErrorResult[] errors)
    {
        Result<TDataResponse> result = new();

        foreach (ErrorResult error in errors)
            result.AddError(error);

        return result;
    }

    public bool IsValid => !HasError;
    public bool HasError => Errors.Count > 0;
    public IList<ErrorResult> Errors
    {
        get;
        private set;
    } = new List<ErrorResult>();

    public Result() { }

    public Result(IEnumerable<ErrorResult> errors)
    {
        foreach (ErrorResult error in errors)
            AddError(error);
    }

    public Result(ErrorResult validation) : this() => AddError(validation);

    public Result(string message) : this(new ErrorResult(null, null, message)) { }

    public Result(string property, string message) : this(new ErrorResult(property, null, message)) { }

    public Result(string? code, string? property, string message) : this(new ErrorResult(code, property, message)) { }

    public Result AddError(ErrorResult error)
    {
        Errors.Add(error);
        return this;
    }

    public Result AddError(string message) =>
        AddError(new ErrorResult(null, null, message));

    public Result AddError(string code, string message) =>
        AddError(new ErrorResult(code, null, message));

    public Result AddError(string? code, string? property, string message) =>
        AddError(new ErrorResult(code, property, message));
}

public record Result<TDataResponse> : Result
{
    public TDataResponse Data
    {
        get;
        private set;
    } =
    default!;

    public Result() : base() { }
    public Result(string message) : base(message) { }
    public Result(string property, string message) : base(property, message) { }
    public Result(string? code, string? property, string message) : base(code, property, message) { }

    public Result(TDataResponse data) : base() => Data = data;

    public Result(ErrorResult[] errors) : base()
    {
        foreach (ErrorResult error in errors)
            AddError(error);
    }
}