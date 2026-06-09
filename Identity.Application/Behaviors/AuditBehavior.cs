using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using Identity.Mediator.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse>(
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext,
    ILogger<AuditBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IsCommandRequest())
        {
            return await next();
        }

        var actionName = ResolveActionName();
        Exception? thrown = null;
        TResponse? response = default;
        var success = true;
        string? details = null;

        try
        {
            response = await next();
            if (response is IResult result)
            {
                success = result.Status is ResultStatus.Ok or ResultStatus.Created or ResultStatus.NoContent;
                if (!success && result.Errors is not null)
                {
                    var errors = result.Errors.Where(x => x is not null).ToList();
                    if (errors.Count > 0)
                    {
                        details = string.Join("; ", errors);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            thrown = ex;
            success = false;
            details = ex.Message;
        }

        await SafeLogAsync(actionName, success, details, cancellationToken);

        if (thrown is not null)
        {
            throw thrown;
        }

        return response!;
    }

    private static bool IsCommandRequest()
    {
        return typeof(TRequest)
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }

    private static string ResolveActionName()
    {
        var name = typeof(TRequest).Name;
        if (name.EndsWith("Command", StringComparison.Ordinal))
        {
            name = name[..^"Command".Length];
        }

        return $"command.{name}";
    }

    private async Task SafeLogAsync(string action, bool success, string? details, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = action,
                    Success = success,
                    ActorUserId = currentRequestContext.UserId,
                    ActorLogin = currentRequestContext.Login,
                    IpAddress = currentRequestContext.IpAddress,
                    UserAgent = currentRequestContext.UserAgent,
                    TargetType = typeof(TRequest).Name,
                    Details = details
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to write audit log for {RequestType}", typeof(TRequest).Name);
        }
    }
}
