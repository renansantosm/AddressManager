using AddressManager.Domain.Exceptions;

namespace AddressManager.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; private set; }

    protected Entity()
    { }

    protected Entity(Guid id)
    {
        DomainValidationException.When(id == Guid.Empty, "O Id não pode ser vazio.");

        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
