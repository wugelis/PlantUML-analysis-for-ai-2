using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Domain.Entities;

namespace RentalCarSystem.Infrastructure.Repositories;

public class InMemoryRentalRepository : IRentalRepository
{
    private readonly List<Rental> _rentals = new();

    public Task<Rental?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rental = _rentals.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(rental);
    }

    public Task<IReadOnlyList<Rental>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var rentals = _rentals.Where(r => r.CustomerId == customerId).ToList();
        return Task.FromResult<IReadOnlyList<Rental>>(rentals);
    }

    public Task<IReadOnlyList<Rental>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Rental>>(_rentals.ToList());
    }

    public Task AddAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        _rentals.Add(rental);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        var index = _rentals.FindIndex(r => r.Id == rental.Id);
        if (index >= 0)
        {
            _rentals[index] = rental;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rental = _rentals.FirstOrDefault(r => r.Id == id);
        if (rental != null)
        {
            _rentals.Remove(rental);
        }
        return Task.CompletedTask;
    }

}