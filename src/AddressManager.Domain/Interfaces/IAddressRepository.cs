using AddressManager.Domain.Entities;

namespace AddressManager.Domain.Interfaces;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id);
    Task<Address?> GetByIdAsNoTrackingAsync(Guid id);
    Task<IEnumerable<Address>> GetAllAsync(int pageNumber, int pageSize);
    Task<Address> AddAsync(Address address);
    void Update(Address address);
    void Delete(Address address);
}
