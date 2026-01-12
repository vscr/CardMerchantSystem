using Fee.Application.DTOs;
using FluentValidation;

namespace Fee.Application.Validators;

public class CreateFeeAccrualValidator : AbstractValidator<CreateFeeAccrualDto>
{
    public CreateFeeAccrualValidator()
    {
        RuleFor(x => x.FeeTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir ücret tipi seçiniz");

        RuleFor(x => x.PeriodId)
            .GreaterThan(0).WithMessage("Geçerli bir periyot seçiniz");

        RuleFor(x => x.GrossAmount)
            .GreaterThan(0).WithMessage("Brüt tutar sıfırdan büyük olmalı");

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("İndirim tutarı negatif olamaz")
            .LessThanOrEqualTo(x => x.GrossAmount).WithMessage("İndirim brüt tutardan büyük olamaz");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Vade tarihi boş olamaz");

        RuleFor(x => x.PeriodStart)
            .NotEmpty().WithMessage("Dönem başlangıcı boş olamaz")
            .Matches(@"^\d{6}$").WithMessage("Dönem formatı YYYYMM olmalı");

        RuleFor(x => x.PeriodEnd)
            .NotEmpty().WithMessage("Dönem bitişi boş olamaz")
            .Matches(@"^\d{6}$").WithMessage("Dönem formatı YYYYMM olmalı");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.MerchantId) ||
                       !string.IsNullOrEmpty(x.CardNumber) ||
                       !string.IsNullOrEmpty(x.TerminalId))
            .WithMessage("En az bir sahip bilgisi (MerchantId, CardNumber veya TerminalId) girilmeli");
    }
}