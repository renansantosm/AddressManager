using AddressManager.Domain.Common;
using AddressManager.Domain.Exceptions;

namespace AddressManager.Domain.ValueObjects;

public sealed class City : ValueObject<City>
{
    public const int MaxLength = 30;
    public string Value { get; private set; }

    private City(string value)
    {
        Value = value;
    }

    public static City Create(string value)
    {
        DomainValidationException.When(string.IsNullOrWhiteSpace(value), "O campo Cidade é obrigatório.");
        DomainValidationException.When(value.Length > MaxLength, $"O campo Cidade deve ter no máximo {MaxLength} caracteres.");
        return new City(value);
    }

#pragma warning disable CS8618
    private City() { }
#pragma warning restore CS8618

    protected override bool EqualsCore(City other)
    {
        return Value == other.Value;
    }

    protected override int GetHashCodeCore()
    {
        int hashCode = 17;
        hashCode = (hashCode * 317) ^ Value.GetHashCode();
        return hashCode;
    }
}