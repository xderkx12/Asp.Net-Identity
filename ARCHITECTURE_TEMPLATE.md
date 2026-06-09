# Microservice Architecture Template (Domain-Agnostic)

This document describes a reusable architecture for building .NET microservices with the same structure, coding style, and cross-cutting behavior as this project.

Use it as a baseline for any domain (`Billing`, `Catalog`, `Orders`, `Notifications`, etc.) by replacing domain entities, use-cases, and API contracts.

---

## 1) Architectural Style

- Clean Architecture with explicit layers:
  - `API` (delivery / transport)
  - `Application` (use-cases, contracts, pipelines)
  - `Domain` (business entities and invariants)
  - `Infrastructure` (persistence, security, external adapters)
  - `Mediator` (CQRS abstractions for commands/queries)
- CQRS-style request handling via MediatR.
- Unified operation result model via `Ardalis.Result`.
- FluentValidation + MediatR pipeline behaviors for validation and exception mapping.

Key rule: **outer layers depend on inner layers, never vice versa**.

---

## 2) Solution Structure (Template)

For a service named `<ServiceName>`:

```text
<ServiceName>.API/
<ServiceName>.Application/
<ServiceName>.Domain/
<ServiceName>.Infrastructure/
<ServiceName>.Mediator/
```

### `<ServiceName>.API`

- Purpose: HTTP transport layer only.
- Typical folders:
  - `Controllers/` - thin endpoints
  - `Extensions/` - mapping `Result -> IActionResult`
  - `Responses/` - API response envelope DTOs
  - `Program.cs` - DI composition, middleware, auth, swagger
- Rules:
  - No business logic.
  - No direct EF access.
  - Controllers call mediator/use-cases only.

### `<ServiceName>.Application`

- Purpose: application use-cases and orchestration.
- Typical folders:
  - `Features/<BoundedContext>/<Commands|Queries>/<UseCase>/`
    - `<UseCase>Command|Query.cs`
    - `<UseCase>Handler.cs`
    - `<UseCase>Validator.cs`
  - `Abstractions/`
    - `Persistence/` (repository interfaces)
    - `Security/` (token/hash abstractions)
    - `Services/` (application services contracts)
  - `Services/` (implementation of orchestration logic)
  - `Behaviors/` (MediatR pipeline behaviors)
  - `Common/Exceptions/` (domain/application exceptions)
  - `DependencyInjection.cs` (registrations)
- Rules:
  - Contains business use-case flow.
  - Depends on `Domain` and `Mediator`.
  - Contains interfaces, not infrastructure implementation details.

### `<ServiceName>.Domain`

- Purpose: core domain model.
- Typical folders:
  - `Common/` - base entity/value object abstractions
  - `Entities/` - aggregate roots and entities
- Rules:
  - No EF, no ASP.NET, no infrastructure dependencies.
  - Encapsulate invariants in methods/properties.

### `<ServiceName>.Infrastructure`

- Purpose: technical implementations.
- Typical folders:
  - `Persistence/`
    - `IdentityDbContext` (or `<ServiceName>DbContext`)
    - `Configurations/` (`IEntityTypeConfiguration<T>`)
    - `Repositories/` (EF implementations of app abstractions)
    - `Migrations/`
  - `Security/` - token generators, password hashers, etc.
  - `DependencyInjection.cs`
- Rules:
  - Implements interfaces from `Application`.
  - No controller/UI logic.

### `<ServiceName>.Mediator`

- Purpose: shared CQRS contracts.
- Typical folders:
  - `Abstractions/ICommand.cs`
  - `Abstractions/IQuery.cs`
- Rules:
  - Minimal surface, no domain coupling.

---

## 3) Dependency Graph (Must Keep)

- `API -> Application, Infrastructure, Mediator`
- `Application -> Domain, Mediator`
- `Infrastructure -> Application, Domain`
- `Domain -> (none)`
- `Mediator -> (none)`

This graph is mandatory for all new services.

---

## 4) Request Processing Pipeline (Standard)

1. Controller receives HTTP request DTO.
2. Controller sends MediatR command/query.
3. Pipeline behaviors execute:
   - `ExceptionHandlingBehavior`
   - `ValidationBehavior`
   - `TransactionBehavior` (for commands only)
4. Handler/ApplicationService performs use-case.
5. Repository/Infrastructure persists/reads data.
6. Handler returns `Result<T>`.
7. API `ResultExtensions` maps to HTTP response.

Keep pipeline behavior-based cross-cutting concerns centralized. Do not duplicate `try/catch` or validation in each handler.
For write operations, keep transaction handling inside pipeline behavior, not in handlers/controllers.

---

## 5) Error & Validation Strategy

- Use `Ardalis.Result` for all handler results.
- Validation:
  - FluentValidation validators per command/query.
  - Validation behavior maps failures to `Result.Invalid`.
- Exceptions:
  - Throw semantic exceptions in Application layer (`ConflictException`, `UnauthorizedException`, etc.).
  - `ExceptionHandlingBehavior` maps exceptions to `Result` statuses.
- API layer:
  - One response mapping point (`ResultExtensions`) for HTTP codes and response body shape.

Recommended response envelope:

- `success`
- `statusCode`
- `message`
- `errors`
- `data`

---

## 6) Persistence Pattern

- One DbContext per microservice.
- One repository interface per aggregate/use-case need (in `Application/Abstractions/Persistence`).
- One repository implementation in `Infrastructure/Persistence/Repositories`.
- Entity configuration classes in `Infrastructure/Persistence/Configurations`.
- Migrations in infrastructure project.
- Apply migrations on startup (or via deployment pipeline, depending on ops policy).
- Use `IUnitOfWork` abstraction and infrastructure-backed transaction scope for command handling.

For token/session-like data:

- Store token hashes, not plaintext tokens.
- Add indexes for high-cardinality lookup fields (e.g. refresh token hash).

---

## 7) Security Baseline

- Access tokens: JWT.
- Refresh tokens: `HttpOnly` + `Secure` cookies for browser clients.
- Validate issuer, audience, signature, lifetime.
- For protected endpoints:
  - Require `[Authorize]`.
  - Use current user id from JWT (`sub`/`nameidentifier`) on server side.
- Do not trust client-provided user ids for ownership-sensitive operations.

---

## 8) API Design Rules

- Controllers are thin and orchestrate transport only.
- Keep endpoint naming consistent:
  - `POST /api/<resource>/create`
  - `POST /api/<resource>/update`
  - `GET /api/<resource>/{id}`
  - etc. (or strict REST nouns, if team standard requires).
- Keep DTOs explicit; avoid leaking domain entities over API.
- Swagger must include Bearer auth definition for secure endpoint testing.

---

## 9) Coding Style & Conventions

- Use file-scoped namespaces.
- Use `sealed` for handlers/services unless extension is intended.
- Prefer constructor injection.
- Keep methods small and intention-revealing.
- Keep business decisions in Application/Domain, not in controllers.
- Avoid static/global mutable state.
- Prefer async/await end-to-end for I/O paths.
- Require `CancellationToken` parameter in all public async methods.
- Always propagate `CancellationToken` through the full async chain:
  - controller -> mediator -> handler -> service -> repository -> EF/external clients.
- Avoid `Console.WriteLine` in production code; use structured logging.
- Name classes by responsibility:
  - `<UseCase>Command`, `<UseCase>Query`
  - `<UseCase>Handler`
  - `<UseCase>Validator`
  - `<Entity>Repository`

---

## 10) Reusable Feature Skeleton

For new domain feature `<Feature>`:

1. Create command/query record in `Application/Features/...`.
2. Create validator in same feature folder.
3. Create handler in same feature folder.
4. Add/extend application service if orchestration is non-trivial.
5. Add/extend repository interface in Application abstractions.
6. Implement repository in Infrastructure.
7. Add controller endpoint in API and map to mediator.
8. Add/update EF configuration and migration if schema changed.
9. Ensure `ResultExtensions` maps required statuses.

---

## 11) New Microservice Bootstrap Checklist

- Create 5 projects (`API`, `Application`, `Domain`, `Infrastructure`, `Mediator`).
- Wire references according to dependency graph.
- Add baseline packages:
  - MediatR
  - FluentValidation
  - Ardalis.Result
  - EF Core + provider
  - JWT auth
  - Swagger/Swashbuckle
- Add baseline folders from this template.
- Implement:
  - `ICommand<T>`, `IQuery<T>`
  - `ValidationBehavior`, `ExceptionHandlingBehavior`
  - `ResultExtensions` response mapper
  - DbContext + repository base + migrations
  - `Program.cs` composition root

Then only domain-specific parts change:

- entities
- feature commands/queries
- repository methods
- controller endpoints

All cross-cutting architecture remains the same.

---

## 12) What Must Stay Generic Across Domains

These parts should be copied nearly unchanged between services:

- Project split and dependency graph
- MediatR + CQRS abstractions
- Pipeline behaviors
- Result-to-HTTP mapping pattern
- DI composition pattern
- Persistence layering pattern
- Security middleware and token validation flow
- Code conventions and naming scheme

This is the reusable platform layer for all future microservices.

---

## 13) Reusable Code (Copy-Paste Ready)

Below are baseline classes/abstractions that are domain-agnostic and can be reused almost unchanged in other microservices.

### 13.1 CQRS Contracts (`Mediator` layer)

```csharp
using Ardalis.Result;
using MediatR;

namespace <ServiceName>.Mediator.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
```

### 13.2 Base Entity (`Domain/Common`)

```csharp
namespace <ServiceName>.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
}
```

### 13.3 Generic Repository Contract (`Application/Abstractions/Persistence`)

```csharp
using <ServiceName>.Domain.Common;

namespace <ServiceName>.Application.Abstractions.Persistence;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 13.4 Generic EF Repository Base (`Infrastructure/Persistence/Repositories`)

```csharp
using <ServiceName>.Application.Abstractions.Persistence;
using <ServiceName>.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace <ServiceName>.Infrastructure.Persistence.Repositories;

public class BaseEntityRepository<TEntity>(<ServiceName>DbContext dbContext) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly <ServiceName>DbContext DbContext = dbContext;
    protected readonly DbSet<TEntity> DbSet = dbContext.Set<TEntity>();

    public virtual Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => DbSet.AddAsync(entity, cancellationToken).AsTask();

    public virtual void Update(TEntity entity) => DbSet.Update(entity);
    public virtual void Remove(TEntity entity) => DbSet.Remove(entity);
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => DbContext.SaveChangesAsync(cancellationToken);
}
```

### 13.5 Validation Pipeline Behavior (`Application/Behaviors`)

```csharp
using Ardalis.Result;
using FluentValidation;
using MediatR;

namespace <ServiceName>.Application.Behaviors;

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
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(x => x.Errors).Where(x => x is not null).ToList();

        if (failures.Count == 0)
            return await next();

        var errors = failures.Select(f => new ValidationError
        {
            Identifier = f.PropertyName,
            ErrorMessage = f.ErrorMessage
        }).ToList();

        object invalidResult = typeof(TResponse) switch
        {
            { } t when t == typeof(Result) => Result.Invalid(errors),
            { } t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>) =>
                CreateGenericInvalidResult(t, errors),
            _ => throw new InvalidOperationException($"Only Ardalis.Result is supported. Got: {typeof(TResponse).Name}")
        };

        return (TResponse)invalidResult;
    }

    private static object CreateGenericInvalidResult(Type responseType, List<ValidationError> errors)
    {
        var genericArg = responseType.GetGenericArguments()[0];
        var method = typeof(Result<>)
            .MakeGenericType(genericArg)
            .GetMethod(nameof(Result<object>.Invalid), [typeof(List<ValidationError>)]);

        return method?.Invoke(null, [errors])
               ?? throw new InvalidOperationException("Unable to create invalid result.");
    }
}
```

### 13.6 Exception Pipeline Behavior (`Application/Behaviors`)

```csharp
using Ardalis.Result;
using MediatR;

namespace <ServiceName>.Application.Behaviors;

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
        catch (Exception ex)
        {
            // Optional: map custom exceptions to Conflict/Unauthorized/NotFound/etc.
            return CreateResponse(Result.Error(ex.Message));
        }
    }

    private static TResponse CreateResponse(Result result)
    {
        object response = typeof(TResponse) switch
        {
            { } t when t == typeof(Result) => result,
            { } t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>) =>
                CreateGenericResponse(t, result),
            _ => throw new InvalidOperationException($"Only Ardalis.Result is supported. Got: {typeof(TResponse).Name}")
        };

        return (TResponse)response;
    }

    private static object CreateGenericResponse(Type responseType, Result result)
    {
        var genericArg = responseType.GetGenericArguments()[0];
        var method = typeof(Result<>)
            .MakeGenericType(genericArg)
            .GetMethod(nameof(Result<object>.Error), [typeof(IEnumerable<string>)]);

        return method?.Invoke(null, [result.Errors])
               ?? throw new InvalidOperationException("Unable to create error result.");
    }
}
```

### 13.7 Base API Controller (`API/Controllers`)

```csharp
using Ardalis.Result;
using <ServiceName>.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace <ServiceName>.Api.Controllers;

[ApiController]
public abstract class BaseController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    protected Task<Result<TResponse>> SendRequestAsync<TResponse>(
        IRequest<Result<TResponse>> request,
        CancellationToken cancellationToken = default)
        => _mediator.Send(request, cancellationToken);

    protected async Task<IActionResult> HandleRequestAsync<TResponse>(
        IRequest<Result<TResponse>> request,
        CancellationToken cancellationToken = default)
    {
        var response = await SendRequestAsync(request, cancellationToken);
        return response.ToActionResult();
    }
}
```

### 13.8 Swagger Baseline (`API/Program.cs`)

```csharp
using Microsoft.OpenApi;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "<ServiceName> API v1");
        options.RoutePrefix = "swagger";
    });
}
```

### 13.9 `Result` to HTTP Mapping (`API/Extensions`)

```csharp
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;

namespace <ServiceName>.Api.Extensions;

internal static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        var statusCode = result.Status switch
        {
            ResultStatus.Ok => 200,
            ResultStatus.Created => 201,
            ResultStatus.Invalid => 400,
            ResultStatus.Unauthorized => 401,
            ResultStatus.Forbidden => 403,
            ResultStatus.NotFound => 404,
            ResultStatus.Conflict => 409,
            _ => 500
        };

        return new ObjectResult(result) { StatusCode = statusCode };
    }
}
```

### 13.10 DI Registration Baseline (`Application/DependencyInjection`)

```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace <ServiceName>.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
```

### 13.11 Infrastructure DI Baseline (`Infrastructure/DependencyInjection`)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace <ServiceName>.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("<ServiceName>Database")
                               ?? throw new InvalidOperationException("Connection string is missing.");

        services.AddDbContext<<ServiceName>DbContext>(opt => opt.UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(BaseEntityRepository<>));

        return services;
    }
}
```

### 13.12 Transaction Abstractions (`Application/Abstractions/Persistence`)

```csharp
namespace <ServiceName>.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

public interface ITransactionScope : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
```

### 13.13 Transaction Pipeline Behavior (`Application/Behaviors`)

```csharp
using <ServiceName>.Application.Abstractions.Persistence;
using <ServiceName>.Mediator.Abstractions;
using MediatR;

namespace <ServiceName>.Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var isCommand = typeof(TRequest)
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));

        if (!isCommand)
            return await next();

        await using var tx = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();
            await tx.CommitAsync(cancellationToken);
            return response;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
```

### 13.14 EF Unit Of Work Implementation (`Infrastructure/Persistence`)

```csharp
using <ServiceName>.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace <ServiceName>.Infrastructure.Persistence;

public sealed class EfUnitOfWork(<ServiceName>DbContext dbContext) : IUnitOfWork
{
    public async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransactionScope(transaction);
    }

    private sealed class EfTransactionScope(IDbContextTransaction transaction) : ITransactionScope
    {
        public Task CommitAsync(CancellationToken cancellationToken = default) => transaction.CommitAsync(cancellationToken);
        public Task RollbackAsync(CancellationToken cancellationToken = default) => transaction.RollbackAsync(cancellationToken);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
```
