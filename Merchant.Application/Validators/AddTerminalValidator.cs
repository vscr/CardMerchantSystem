using Merchant.Application.Commands;
using FluentValidation;

namespace Merchant.Application.Validators;

public class AddTerminalValidator : AbstractValidator<AddTerminalCommand>
{
    public AddTerminalValidator()
    {
        RuleFor(x => x.MerchantId)
            .NotEmpty().WithMessage("Üye işyeri ID boş olamaz");

        RuleFor(x => x.Dto.TerminalTypeId)
            .InclusiveBetween(1, 5).WithMessage("Geçersiz terminal tipi");

        RuleFor(x => x.Dto.SerialNumber)
            .MaximumLength(50).WithMessage("Seri numarası en fazla 50 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Dto.SerialNumber));

        RuleFor(x => x.Dto.Model)
            .MaximumLength(50).WithMessage("Model en fazla 50 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Dto.Model));

        RuleFor(x => x.Dto.Location)
            .MaximumLength(200).WithMessage("Konum en fazla 200 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Dto.Location));
    }
}