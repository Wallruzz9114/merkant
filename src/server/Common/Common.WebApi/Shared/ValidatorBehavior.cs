using Common.EventBus.Utils;
using Common.WebApi.Results;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.WebApi.Shared;

public class ValidatorBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result, new()
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    private TResponse Errors(IEnumerable<ValidationFailure> errors)
    {
        TResponse response = new();

        foreach (ValidationFailure error in errors)
            response.AddError(error.ErrorCode, error.PropertyName, error.ErrorMessage);

        return response;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string typeName = request.GetGenericTypeName();

        _logger.LogInformation("----- Fail Fast Validations: Validating command {CommandType}", typeName);

        List<ValidationFailure> errors = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .ToList();

        if (errors.Any())
        {
            _logger.LogWarning("----- Fail Fast Validations: Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, errors);

            return Errors(errors);
        }

        _logger.LogInformation("----- Fail Fast Validations: Validated command {CommandType}", typeName);

        return await next();
    }
}