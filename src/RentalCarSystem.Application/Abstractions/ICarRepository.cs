using RentalCarSystem.Domain.Entities;
using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Application.Abstractions;

public interface ICarRepository
{
    Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Car>> GetAvailableCarsByTypeAsync(CarType carType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Car car, CancellationToken cancellationToken = default);
    Task UpdateAsync(Car car, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}