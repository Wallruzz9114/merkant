using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Common.Grpc;

public class GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{
    private readonly ILogger<GrpcExceptionInterceptor> _logger = logger;

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        AsyncUnaryCall<TResponse> call = continuation(request, context);
        return new AsyncUnaryCall<TResponse>(
            HandleResponse(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
    }

    private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> t)
    {
        try
        {
            TResponse? response = await t;
            return response;
        }
        catch (RpcException e)
        {
            _logger.LogError("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
            return default!;
        }
    }
}