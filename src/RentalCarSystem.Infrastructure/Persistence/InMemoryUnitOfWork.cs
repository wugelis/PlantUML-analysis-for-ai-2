using RentalCarSystem.Application.Abstractions;

namespace RentalCarSystem.Infrastructure.Persistence;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In-memory implementation doesn't need to save changes
        return Task.CompletedTask;
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // In-memory implementation doesn't support transactions
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        // In-memory implementation doesn't support transactions
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        // In-memory implementation doesn't support transactions
        return Task.CompletedTask;
    }
}