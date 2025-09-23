using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Domain.Entities;

namespace RentalCarSystem.Infrastructure.Repositories;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var customer = _customers.FirstOrDefault(c => c.UserId == userId);
        return Task.FromResult(customer);
    }

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Customer>>(_customers.ToList());
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _customers.Add(customer);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var index = _customers.FindIndex(c => c.Id == customer.Id);
        if (index >= 0)
        {
            _customers[index] = customer;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer != null)
        {
            _customers.Remove(customer);
        }
        return Task.CompletedTask;
    }
}