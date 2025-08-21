using AddressManager.Domain.Common;
using AddressManager.Domain.Exceptions;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace AddressManager.Domain.Value_Objects;

public sealed class ZipCode : ValueObject<ZipCode>
{
    private static readonly Regex ZipCodeRegex = new(@"^\d{5}-?\d{3}$");
    public string Value { get; private set; }

    private ZipCode(string value)
    {
        Value = value;
    }

    public static ZipCode Create(string value)
    {
        DomainValidationException.When(string.IsNullOrWhiteSpace(value), "O campo CEP é obrigatório.");
        DomainValidationException.When(!ZipCodeRegex.IsMatch(value), "Formato de CEP inválido. Digite no formato 12345-678 ou 12345678.");

        string normalizedZipCode = new string(value.Where(char.IsDigit).ToArray());

        return new ZipCode(normalizedZipCode);
    }

    protected ZipCode() { }

    protected override bool EqualsCore(ZipCode other)
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
