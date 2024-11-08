namespace Common.WebApi.Results;

public record ErrorResult
{
    public string? Code { get; set; }
    public string? Property { get; set; }
    public string Message { get; set; } = null!;

    public ErrorResult(string? code, string? property, string message)
    {
        Code = code ?? "Error";
        Property = property;
        Message = message;
    }

    public ErrorResult(string message)
    {
        Code = "Error";
        Property = null;
        Message = message;
    }
}