using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Domain.Entities;
using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Infrastructure.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private readonly List<Car> _cars = new();

    public InMemoryCarRepository()
    {
        // 初始化一些測試車輛
        SeedData();
    }

    private void SeedData()
    {
        _cars.Add(Car.Create("Toyota Camry", CarType.Car));
        _cars.Add(Car.Create("Honda CR-V", CarType.SUV));
        _cars.Add(Car.Create("Ford F-150", CarType.Truck));
        _cars.Add(Car.Create("BMW Z4", CarType.SportsCar));
        _cars.Add(Car.Create("Nissan Altima", CarType.Car));
        _cars.Add(Car.Create("Mazda CX-5", CarType.SUV));
    }

    public Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var car = _cars.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(car);
    }

    public Task<IReadOnlyList<Car>> GetAvailableCarsByTypeAsync(CarType carType, CancellationToken cancellationToken = default)
    {
        var cars = _cars.Where(c => c.CarType == carType && c.IsAvailable).ToList();
        return Task.FromResult<IReadOnlyList<Car>>(cars);
    }

    public Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Car>>(_cars.ToList());
    }

    public Task AddAsync(Car car, CancellationToken cancellationToken = default)
    {
        _cars.Add(car);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Car car, CancellationToken cancellationToken = default)
    {
        var index = _cars.FindIndex(c => c.Id == car.Id);
        if (index >= 0)
        {
            _cars[index] = car;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var car = _cars.FirstOrDefault(c => c.Id == id);
        if (car != null)
        {
            _cars.Remove(car);
        }
        return Task.CompletedTask;
    }
}