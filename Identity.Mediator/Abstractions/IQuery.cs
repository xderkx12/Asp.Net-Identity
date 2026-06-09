using Ardalis.Result;
using MediatR;

namespace Identity.Mediator.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
