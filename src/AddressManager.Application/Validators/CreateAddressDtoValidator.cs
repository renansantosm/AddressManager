using AddressManager.Application.DTOs;
using FluentValidation;

namespace AddressManager.Application.Validators;

public sealed class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressDtoValidator()
    {
        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("O campo CEP é obrigatório.")
            .Matches(@"^(\d{8}|\d{5}-\d{3})$").WithMessage("Formato de CEP inválido. Digite no formato 12345-678 ou 12345678.");

        RuleFor(x => x.Number)
            .MaximumLength(10).WithMessage("O campo Número deve ter no máximo 10 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Number));

        RuleFor(x => x.Complement)
            .MaximumLength(50).WithMessage("O campo Complemento deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Complement));

        RuleFor(x => x.Reference)
            .MaximumLength(50).WithMessage("O campo Referência deve ter no máximo 50 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Reference));
    }
}
