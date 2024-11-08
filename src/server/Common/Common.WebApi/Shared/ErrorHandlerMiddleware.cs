using System.Net;
using System.Text.Json;
using Common.WebApi.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Common.WebApi.Shared;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            HttpResponse response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = ex switch
            {
                SecurityTokenException => (int)HttpStatusCode.Unauthorized,
                TaskCanceledException => 499,
                OperationCanceledException => 499,
                _ => (int)HttpStatusCode.InternalServerError
            };

            string result = JsonSerializer.Serialize(Result.Fail(ex.Message));
            await response.WriteAsync(result);
        }
    }
}