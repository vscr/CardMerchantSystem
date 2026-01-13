using Statement.Application.DTOs;
using FluentValidation;

namespace Statement.Application.Validators;

public class AddStatementItemValidator : AbstractValidator<AddStatementItemDto>
{
    public AddStatementItemValidator()
    {
        RuleFor(x => x.StatementId)
            .NotEmpty().WithMessage("Ekstre ID boş olamaz");

        RuleFor(x => x.ItemTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir kalem tipi seçiniz");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("İşlem tarihi boş olamaz");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama boş olamaz")
            .MaximumLength(200).WithMessage("Açıklama en fazla 200 karakter olabilir");

        RuleFor(x => x.Amount)
            .NotEqual(0).WithMessage("Tutar sıfır olamaz");

        RuleFor(x => x.InstallmentNumber)
            .GreaterThan(0).When(x => x.InstallmentNumber.HasValue)
            .WithMessage("Taksit numarası sıfırdan büyük olmalı");

        RuleFor(x => x.TotalInstallments)
            .GreaterThanOrEqualTo(x => x.InstallmentNumber)
            .When(x => x.InstallmentNumber.HasValue && x.TotalInstallments.HasValue)
            .WithMessage("Toplam taksit, taksit numarasından küçük olamaz");
    }
}