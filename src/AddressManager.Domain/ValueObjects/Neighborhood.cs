using AddressManager.Domain.Common;
using AddressManager.Domain.Exceptions;

namespace AddressManager.Domain.ValueObjects;

public sealed class Neighborhood : ValueObject<Neighborhood>
{
    public const int MaxLength = 60;
    public string Value { get; private set; }

    private Neighborhood(string value)
    {
        Value = value;
    }

    public static Neighborhood Create(string value)
    {
        DomainValidationException.When(string.IsNullOrWhiteSpace(value), "O campo Bairro é obrigatório.");
        DomainValidationException.When(value.Length > MaxLength, $"O campo Bairro deve ter no máximo {MaxLength} caracteres.");
        return new Neighborhood(value);
    }

    #pragma warning disable CS8618
    private Neighborhood() { }
    #pragma warning restore CS8618

    protected override bool EqualsCore(Neighborhood other)
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
