using AddressManager.Domain.Common;
using AddressManager.Domain.Exceptions;

namespace AddressManager.Domain.ValueObjects;

public sealed class Street : ValueObject<Street>
{
    public const int MaxLength = 100;
    public string Value { get; private set; }

    private Street(string value)
    {
        Value = value;
    }

    public static Street Create(string value)
    {
        DomainValidationException.When(string.IsNullOrWhiteSpace(value), "O campo Rua é obrigatório.");
        DomainValidationException.When(value.Length > MaxLength, $"O campo Rua deve ter no máximo {MaxLength} caracteres.");

        return new Street(value);
    }
    protected Street() { } 

    protected override bool EqualsCore(Street other)
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
