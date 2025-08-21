namespace AddressManager.Domain.Exceptions;

public class AddressNotFoundException : Exception
{
    public AddressNotFoundException(string error) : base(error) { }
}
