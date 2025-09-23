using AddressManager.Domain.Entities;
using AddressManager.Domain.Interfaces;
using AddressManager.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Infra.Data.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _context;
    public AddressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Address>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _context.Addresses
                        .AsNoTracking()
                        .OrderBy(x => x.Id)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
    }

    public async Task<Address?> GetByIdAsync(Guid id)
    {
        return await _context.Addresses.FindAsync(id);
    }


    public Task<Address?> GetByIdAsNoTrackingAsync(Guid id)
    {
        return _context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Address> AddAsync(Address address)
    {
        var adress = await _context.Addresses.AddAsync(address);
        return address;
    }

    public async Task UpdateAsync(Address address)
    {
        _context.Addresses.Update(address);
    }

    public async Task DeleteAsync(Address address)
    {
        _context.Addresses.Remove(address);
    }
}
