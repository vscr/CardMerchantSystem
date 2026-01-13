using Statement.Application.DTOs;
using FluentValidation;

namespace Statement.Application.Validators;

public class CreateStatementPeriodConfigValidator : AbstractValidator<CreateStatementPeriodConfigDto>
{
    public CreateStatementPeriodConfigValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Kart numarası boş olamaz")
            .Length(16).WithMessage("Kart numarası 16 haneli olmalı")
            .Matches(@"^\d+$").WithMessage("Kart numarası sadece rakam içermeli");

        RuleFor(x => x.StatementDay)
            .InclusiveBetween(1, 28).WithMessage("Kesim günü 1-28 arasında olmalı");

        RuleFor(x => x.PaymentDueDays)
            .InclusiveBetween(1, 30).WithMessage("Ödeme vadesi 1-30 gün arasında olmalı");

        RuleFor(x => x.InterestRate)
            .InclusiveBetween(0, 100).WithMessage("Faiz oranı 0-100 arasında olmalı");

        RuleFor(x => x.CashAdvanceInterestRate)
            .InclusiveBetween(0, 100).WithMessage("Nakit avans faiz oranı 0-100 arasında olmalı");

        RuleFor(x => x.MinimumPaymentRate)
            .InclusiveBetween(1, 100).WithMessage("Minimum ödeme oranı 1-100 arasında olmalı");

        RuleFor(x => x.MinimumPaymentAmount)
            .GreaterThan(0).WithMessage("Minimum ödeme tutarı sıfırdan büyük olmalı");
    }
}