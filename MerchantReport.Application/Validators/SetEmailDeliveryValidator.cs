using MerchantReport.Application.DTOs;
using FluentValidation;

namespace MerchantReport.Application.Validators;

public class SetEmailDeliveryValidator : AbstractValidator<SetEmailDeliveryDto>
{
    public SetEmailDeliveryValidator()
    {
        RuleFor(x => x.ConfigId)
            .NotEmpty().WithMessage("Ayar ID boş olamaz");

        RuleFor(x => x.Recipients)
            .NotEmpty().WithMessage("Alıcı listesi boş olamaz")
            .MaximumLength(500).WithMessage("Alıcı listesi en fazla 500 karakter olabilir");
    }
}