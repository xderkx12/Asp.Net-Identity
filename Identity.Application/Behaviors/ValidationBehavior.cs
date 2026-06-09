using Ardalis.Result;
using FluentValidation;
using MediatR;

namespace Identity.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var errors = failures
            .Select(f => new ValidationError
            {
                Identifier = f.PropertyName,
                ErrorMessage = f.ErrorMessage
            })
            .ToList();

        object invalidResult = typeof(TResponse) switch
        {
            { } responseType when responseType == typeof(Result) => Result.Invalid(errors),
            { } responseType when responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>) =>
                CreateGenericInvalidResult(responseType, errors),
            _ => throw new InvalidOperationException(
                $"ValidationBehavior supports only Ardalis.Result responses. Current response type: {typeof(TResponse).Name}")
        };

        return (TResponse)invalidResult;
    }

    private static object CreateGenericInvalidResult(Type responseType, List<ValidationError> errors)
    {
        var resultType = responseType.GetGenericArguments()[0];
        var method = typeof(Result<>)
            .MakeGenericType(resultType)
            .GetMethod(nameof(Result<object>.Invalid), [typeof(List<ValidationError>)]);

        return method?.Invoke(null, [errors])
               ?? throw new InvalidOperationException("Unable to build invalid generic result.");
    }
}
