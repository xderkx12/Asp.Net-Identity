using Ardalis.Result;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Behaviors;

public sealed class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ConflictException exception)
        {
            return CreateResponse(Result.Conflict(exception.Message), ResultStatus.Conflict);
        }
        catch (UnauthorizedException exception)
        {
            return CreateResponse(Result.Unauthorized([exception.Message]), ResultStatus.Unauthorized);
        }
        catch (NotFoundException exception)
        {
            return CreateResponse(Result.NotFound(exception.Message), ResultStatus.NotFound);
        }
        catch (ForbiddenException exception)
        {
            return CreateResponse(Result.Forbidden([exception.Message]), ResultStatus.Forbidden);
        }
        catch (Exception exception)
        {
            return CreateResponse(Result.Error(exception.Message), ResultStatus.Error);
        }
    }

    private static TResponse CreateResponse(Result result, ResultStatus status)
    {
        object response = typeof(TResponse) switch
        {
            { } responseType when responseType == typeof(Result) => result,
            { } responseType when responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>) =>
                CreateGenericResponse(responseType, result, status),
            _ => throw new InvalidOperationException(
                $"ExceptionHandlingBehavior supports only Ardalis.Result responses. Current response type: {typeof(TResponse).Name}")
        };

        return (TResponse)response;
    }

    private static object CreateGenericResponse(Type responseType, Result result, ResultStatus status)
    {
        var resultType = responseType.GetGenericArguments()[0];
        var methodName = status switch
        {
            ResultStatus.Conflict => nameof(Result<object>.Conflict),
            ResultStatus.Unauthorized => nameof(Result<object>.Unauthorized),
            ResultStatus.NotFound => nameof(Result<object>.NotFound),
            ResultStatus.Forbidden => nameof(Result<object>.Forbidden),
            _ => nameof(Result<object>.Error)
        };

        var method = typeof(Result<>)
            .MakeGenericType(resultType)
            .GetMethod(methodName, [typeof(IEnumerable<string>)]);

        return method?.Invoke(null, [result.Errors])
               ?? throw new InvalidOperationException("Unable to build generic error result.");
    }
}
