using AddressManager.Domain.Common;
using AddressManager.Domain.Exceptions;

namespace AddressManager.Domain.Value_Objects;

public sealed class State : ValueObject<State>
{
    public const int MaxLength = 25;
    public string Name { get; private set; }
    public string Abbreviation { get; private set; }
    private State(string name, string abbreviation)
    {
        Name = name;
        Abbreviation = abbreviation;
    }
    protected State() { }

    public static State Create(string name, string abbreviation)
    {
        DomainValidationException.When(string.IsNullOrWhiteSpace(name), "O campo Nome do Estado é obrigatório.");
        DomainValidationException.When(name.Length > MaxLength, $"O campo Nome do Estado deve ter no máximo {MaxLength} caracteres.");
        DomainValidationException.When(string.IsNullOrWhiteSpace(abbreviation), "O campo Sigla do Estado é obrigatório.");
        DomainValidationException.When(abbreviation.Length != 2, "A sigla do estado deve ter exatamente 2 caracteres.");

        return new State(name, abbreviation);
    }

    protected override bool EqualsCore(State other)
    {
        return Name == other.Name && Abbreviation == other.Abbreviation;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Abbreviation.GetHashCode();
            return hashCode;
        }
    }
}
