using AddressManager.Application.Models;

namespace AddressManager.Application.Interfaces;

public interface IViaCepClient
{
    Task<ViaCepData> GetAddressByZipCodeAsync(string zipCode);
}
