using Accounting.Application.DTOs;
using FluentValidation;

namespace Accounting.Application.Validators;

public class CreateJournalEntryValidator : AbstractValidator<CreateJournalEntryDto>
{
    public CreateJournalEntryValidator()
    {
        RuleFor(x => x.EntryDate)
            .NotEmpty().WithMessage("Fiş tarihi boş olamaz");

        RuleFor(x => x.TransactionTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir işlem tipi seçiniz");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama boş olamaz")
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir");

        RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("En az bir satır eklenmeli")
            .Must(lines => lines.Count >= 2).WithMessage("En az 2 satır gerekli (borç ve alacak)");

        RuleFor(x => x.Lines)
            .Must(BeBalanced).WithMessage("Borç ve alacak toplamları eşit olmalı");

        RuleForEach(x => x.Lines).SetValidator(new CreateJournalEntryLineValidator());
    }

    private bool BeBalanced(List<CreateJournalEntryLineDto> lines)
    {
        if (lines == null || !lines.Any()) return false;
        var totalDebit = lines.Sum(l => l.DebitAmount);
        var totalCredit = lines.Sum(l => l.CreditAmount);
        return totalDebit == totalCredit;
    }
}

public class CreateJournalEntryLineValidator : AbstractValidator<CreateJournalEntryLineDto>
{
    public CreateJournalEntryLineValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Hesap seçilmeli");

        RuleFor(x => x.DebitAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Borç tutarı negatif olamaz");

        RuleFor(x => x.CreditAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Alacak tutarı negatif olamaz");

        RuleFor(x => x)
            .Must(x => x.DebitAmount > 0 || x.CreditAmount > 0)
            .WithMessage("Borç veya alacak tutarı girilmeli");

        RuleFor(x => x)
            .Must(x => !(x.DebitAmount > 0 && x.CreditAmount > 0))
            .WithMessage("Aynı satırda hem borç hem alacak olamaz");
    }
}