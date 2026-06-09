using Ardalis.Result;
using MediatR;

namespace Identity.Mediator.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
