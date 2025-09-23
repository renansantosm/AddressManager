namespace AddressManager.Domain.Exceptions;

public class ExternalServiceException : Exception
{
    public ExternalServiceException(string error) : base(error) { }

}
