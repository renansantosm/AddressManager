namespace AddressManager.Application.DTOs;

public record AddressDto(
    Guid Id, 
    string ZipCode, 
    string Street, 
    string? Number, 
    string? Complement, 
    string? Reference, 
    string Neighborhood, 
    string City, 
    string State, 
    string Region);
