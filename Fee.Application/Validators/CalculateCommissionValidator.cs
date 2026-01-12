using Fee.Application.DTOs;
using FluentValidation;

namespace Fee.Application.Validators;

public class CalculateCommissionValidator : AbstractValidator<CalculateCommissionRequestDto>
{
    public CalculateCommissionValidator()
    {
        RuleFor(x => x.MerchantId)
            .NotEmpty().WithMessage("Üye işyeri ID boş olamaz");

        RuleFor(x => x.TransactionAmount)
            .GreaterThan(0).WithMessage("İşlem tutarı sıfırdan büyük olmalı");

        RuleFor(x => x.MCC)
            .Length(4)
            .When(x => !string.IsNullOrEmpty(x.MCC))
            .WithMessage("MCC 4 karakter olmalı");

        RuleFor(x => x.InstallmentCount)
            .InclusiveBetween(1, 36).WithMessage("Taksit sayısı 1-36 arasında olmalı");
    }
}