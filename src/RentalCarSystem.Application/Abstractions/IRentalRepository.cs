using RentalCarSystem.Domain.Entities;

namespace RentalCarSystem.Application.Abstractions;

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Rental>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Rental>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Rental rental, CancellationToken cancellationToken = default);
    Task UpdateAsync(Rental rental, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}