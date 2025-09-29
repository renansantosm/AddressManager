using AddressManager.Domain.Common;
using AddressManager.Domain.Exceptions;
using AddressManager.Domain.Value_Objects;
using AddressManager.Domain.ValueObjects;

namespace AddressManager.Domain.Entities;

public class Address : Entity
{
    public const int MaxNumberLength = 10;
    public const int MaxComplementLength = 50;
    public const int MaxReferenceLength = 50;

    public ZipCode ZipCode { get; private set; }
    public Street Street { get; private set; }
    public string? Number { get; private set; }
    public string? Complement { get; private set; }
    public string? Reference { get; private set; }
    public Neighborhood Neighborhood { get; private set; }
    public City City { get; private set; }
    public State State { get; private set; }
    public Region Region { get; private set; }

    public Address(Guid id, ZipCode zipCode, Street street, Neighborhood neighborhood, City city, State state, Region region) : base(id)
    {

        ZipCode = zipCode;
        Street = street;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Region = region;
    }

#pragma warning disable CS8618
    private Address() { }
#pragma warning restore CS8618

    public void UpdateNumber(string? number)
    {
        if (number != null)
        {
            DomainValidationException.When(number.Length > MaxNumberLength, $"O campo Número deve ter no máximo {MaxNumberLength} caracteres.");
        }
        Number = number;
    }

    public void UpdateComplement(string? complement)
    {
        if(complement != null)
        {
            DomainValidationException.When(complement.Length > MaxComplementLength, $"O campo Complemento deve ter no máximo {MaxComplementLength} caracteres.");
        }
        Complement = complement;
    }

    public void UpdateReference(string? reference)
    {
        if(reference != null)
        {
            DomainValidationException.When(reference.Length > MaxReferenceLength, $"O campo Referencia deve ter no máximo {MaxReferenceLength} caracteres.");
        }
        Reference = reference;
    }
}
