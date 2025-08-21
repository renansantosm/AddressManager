namespace AddressManager.Application.DTOs;

public record CreateAddressDto(
    string ZipCode, 
    string? Number, 
    string? Complement, 
    string? Reference);
