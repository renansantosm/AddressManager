namespace AddressManager.Application.Models;

public record ViaCepData(
    string ZipCode,
    string Street,
    string Neighborhood,
    string City,
    string State,
    string StateAbbreviation,
    string Region);
