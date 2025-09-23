using RentalCarSystem.Domain.Entities;

namespace RentalCarSystem.Application.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}