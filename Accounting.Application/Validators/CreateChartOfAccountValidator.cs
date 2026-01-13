using Accounting.Application.DTOs;
using FluentValidation;

namespace Accounting.Application.Validators;

public class CreateChartOfAccountValidator : AbstractValidator<CreateChartOfAccountDto>
{
    public CreateChartOfAccountValidator()
    {
        RuleFor(x => x.AccountCode)
            .NotEmpty().WithMessage("Hesap kodu boş olamaz")
            .MaximumLength(20).WithMessage("Hesap kodu en fazla 20 karakter olabilir")
            .Matches(@"^[0-9\.]+$").WithMessage("Hesap kodu sadece rakam ve nokta içerebilir");

        RuleFor(x => x.AccountName)
            .NotEmpty().WithMessage("Hesap adı boş olamaz")
            .MaximumLength(100).WithMessage("Hesap adı en fazla 100 karakter olabilir");

        RuleFor(x => x.AccountTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir hesap tipi seçiniz");

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 5).WithMessage("Hesap seviyesi 1-5 arasında olmalı");

        RuleFor(x => x.CurrencyCode)
            .Length(3).When(x => !string.IsNullOrEmpty(x.CurrencyCode))
            .WithMessage("Para birimi kodu 3 karakter olmalı");
    }
}