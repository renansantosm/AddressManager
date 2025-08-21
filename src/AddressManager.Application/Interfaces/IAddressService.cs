using AddressManager.Application.DTOs;
using AddressManager.Domain.Entities;

namespace AddressManager.Application.Interfaces;

public interface IAddressService
{
    Task<AddressDto> GetAddressByIdAsync(Guid id);
    Task<IEnumerable<AddressDto>> GetAllAddressesAsync(int pageNumber, int pageSize);
    Task<AddressDto> CreateAddressAsync(CreateAddressDto addressDto);
    Task UpdateAddressAsync(UpdateAddressDto addressDto);
    Task DeleteAddressAsync(Guid id);
}