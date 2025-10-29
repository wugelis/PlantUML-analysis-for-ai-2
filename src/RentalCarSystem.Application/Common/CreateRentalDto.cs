namespace RentalCarSystem.Application.Common;

public record CreateRentalDto(
    Guid CustomerId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate
);