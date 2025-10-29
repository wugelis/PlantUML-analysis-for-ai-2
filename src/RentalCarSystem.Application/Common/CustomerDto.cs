using RentalCarSystem.Domain.ValueObjects;

namespace RentalCarSystem.Application.Common;

public record CustomerDto(
    Guid Id,
    string UserId,
    string Name,
    string Email,
    DateTime CreatedAt
);