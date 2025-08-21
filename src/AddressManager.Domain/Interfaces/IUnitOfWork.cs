namespace AddressManager.Domain.Interfaces;

public interface IUnitOfWork
{
    IAddressRepository AddressRepository { get; }
    Task Commit();
    Task Dispose();
}
