using AddressManager.Application.DTOs;
using FluentValidation;

namespace AddressManager.Application.Validators;

public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O campo Id é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O campo Id não pode ser um GUID vazio.");

        RuleFor(x => x.Reference)
            .MaximumLength(50).WithMessage("O campo Referência deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Reference));

        RuleFor(x => x.Complement)
            .MaximumLength(50).WithMessage("O campo Complemento deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Complement));
    }
}
