namespace RentalCarSystem.Application.Common;

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