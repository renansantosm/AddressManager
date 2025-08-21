using AddressManager.Domain.Interfaces;
using AddressManager.Infra.Data.Context;
using AddressManager.Infra.Data.Repositories;

namespace AddressManager.Infra.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private IAddressRepository? _addressRepo;
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IAddressRepository AddressRepository
    {
        get
        {
            if (_addressRepo is null)
            {
                _addressRepo = new AddressRepository(_context);
            }

            return _addressRepo;
        }
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public async Task Dispose()
    {
        await _context.DisposeAsync();
    }
}
