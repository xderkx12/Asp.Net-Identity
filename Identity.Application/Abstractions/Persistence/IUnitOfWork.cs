namespace Identity.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

