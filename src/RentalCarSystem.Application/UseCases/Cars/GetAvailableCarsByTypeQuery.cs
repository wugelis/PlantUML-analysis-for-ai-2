using RentalCarSystem.Application.Abstractions;
using RentalCarSystem.Application.Common;
using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Application.UseCases.Cars;

public record GetAvailableCarsByTypeQuery(CarType CarType) : IQuery<IReadOnlyList<CarDto>>;

public class GetAvailableCarsByTypeQueryHandler : IQueryHandler<GetAvailableCarsByTypeQuery, IReadOnlyList<CarDto>>
{
    private readonly ICarRepository _carRepository;

    public GetAvailableCarsByTypeQueryHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<IReadOnlyList<CarDto>> Handle(GetAvailableCarsByTypeQuery query, CancellationToken cancellationToken = default)
    {
        var cars = await _carRepository.GetAvailableCarsByTypeAsync(query.CarType, cancellationToken);
        
        return cars.Select(car => new CarDto(
            car.Id,
            car.Model,
            car.CarType,
            car.DailyRate.Amount,
            car.IsAvailable
        )).ToList();
    }
}