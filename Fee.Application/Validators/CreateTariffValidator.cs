using Fee.Application.DTOs;
using FluentValidation;

namespace Fee.Application.Validators;

public class CreateTariffValidator : AbstractValidator<CreateTariffDto>
{
    public CreateTariffValidator()
    {
        RuleFor(x => x.TariffCode)
            .NotEmpty().WithMessage("Tarife kodu boş olamaz")
            .MaximumLength(20).WithMessage("Tarife kodu en fazla 20 karakter olabilir")
            .Matches("^[A-Z0-9_-]+$").WithMessage("Tarife kodu sadece büyük harf, rakam, tire ve alt çizgi içerebilir");

        RuleFor(x => x.TariffName)
            .NotEmpty().WithMessage("Tarife adı boş olamaz")
            .MaximumLength(100).WithMessage("Tarife adı en fazla 100 karakter olabilir");

        RuleFor(x => x.FeeTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir ücret tipi seçiniz");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty().WithMessage("Başlangıç tarihi boş olamaz");

        RuleFor(x => x.EffectiveTo)
            .GreaterThan(x => x.EffectiveFrom)
            .When(x => x.EffectiveTo.HasValue)
            .WithMessage("Bitiş tarihi başlangıç tarihinden büyük olmalı");
    }
}