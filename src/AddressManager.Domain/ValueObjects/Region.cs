using AddressManager.Domain.Common;
using AddressManager.Domain.Exceptions;

namespace AddressManager.Domain.ValueObjects;

public sealed class Region : ValueObject<Region>
{
    public const int MaxLength = 20;
    public string Value { get; private set; }

    private Region(string value)
    {
        Value = value;
    }

    public static Region Create(string value)
    {
        DomainValidationException.When(string.IsNullOrWhiteSpace(value), "O campo Região é obrigatório.");
        DomainValidationException.When(value.Length > MaxLength, $"O campo Região deve ter no máximo {MaxLength} caracteres.");
        return new Region(value);
    }

    #pragma warning disable CS8618
    private Region() { }
    #pragma warning restore CS8618

    protected override bool EqualsCore(Region other)
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
