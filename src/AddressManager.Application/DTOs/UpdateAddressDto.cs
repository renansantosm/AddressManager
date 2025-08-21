namespace AddressManager.Application.DTOs;

public record UpdateAddressDto(
    Guid Id,
    string? Number,
    string? Complement,
    string? Reference);
