namespace RentalCarSystem.Application.Common;

public record CreateCustomerDto(
    string UserId,
    string Name,
    string Email
);