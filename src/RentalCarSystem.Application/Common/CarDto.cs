using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Application.Common;

public record CarDto(
    Guid Id,
    string Model,
    CarType CarType,
    decimal DailyRate,
    bool IsAvailable
);