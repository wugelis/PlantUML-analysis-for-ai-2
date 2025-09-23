using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Application.Common;

public record CustomerDto(
    Guid Id,
    string UserId,
    string Name,
    string Email,
    DateTime CreatedAt
);

public record CarDto(
    Guid Id,
    string Model,
    CarType CarType,
    decimal DailyRate,
    bool IsAvailable
);

public record RentalDto(
    Guid Id,
    Guid CustomerId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate,
    int Days,
    decimal TotalFee,
    string Status,
    DateTime CreatedAt
);

public record CreateCustomerDto(
    string UserId,
    string Name,
    string Email
);

public record CreateRentalDto(
    Guid CustomerId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate
);