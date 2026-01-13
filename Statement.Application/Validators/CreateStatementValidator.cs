using Statement.Application.DTOs;
using FluentValidation;

namespace Statement.Application.Validators;

public class CreateStatementValidator : AbstractValidator<CreateStatementDto>
{
    public CreateStatementValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("Kart numarası boş olamaz")
            .Length(16).WithMessage("Kart numarası 16 haneli olmalı")
            .Matches(@"^\d+$").WithMessage("Kart numarası sadece rakam içermeli");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Müşteri adı boş olamaz")
            .MaximumLength(100).WithMessage("Müşteri adı en fazla 100 karakter olabilir");

        RuleFor(x => x.CustomerEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.CustomerEmail))
            .WithMessage("Geçerli bir e-posta adresi giriniz");

        RuleFor(x => x.PeriodStartDate)
            .NotEmpty().WithMessage("Dönem başlangıç tarihi boş olamaz");

        RuleFor(x => x.PeriodEndDate)
            .NotEmpty().WithMessage("Dönem bitiş tarihi boş olamaz")
            .GreaterThan(x => x.PeriodStartDate).WithMessage("Dönem bitiş tarihi başlangıçtan büyük olmalı");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Son ödeme tarihi boş olamaz")
            .GreaterThan(x => x.PeriodEndDate).WithMessage("Son ödeme tarihi dönem bitişinden büyük olmalı");

        RuleFor(x => x.StatementDay)
            .InclusiveBetween(1, 28).WithMessage("Kesim günü 1-28 arasında olmalı");

        RuleFor(x => x.CreditLimit)
            .GreaterThan(0).WithMessage("Kredi limiti sıfırdan büyük olmalı");

        RuleFor(x => x.InterestRate)
            .InclusiveBetween(0, 100).WithMessage("Faiz oranı 0-100 arasında olmalı");
    }
}