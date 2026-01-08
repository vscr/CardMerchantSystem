using Transaction.Application.Commands;
using FluentValidation;

namespace Transaction.Application.Validators;

public class ProcessTransactionValidator : AbstractValidator<ProcessTransactionCommand>
{
    public ProcessTransactionValidator()
    {
        RuleFor(x => x.Dto.TransactionTypeId)
            .InclusiveBetween(1, 6).WithMessage("Geçersiz işlem tipi");

        RuleFor(x => x.Dto.Amount)
            .GreaterThan(0).WithMessage("İşlem tutarı sıfırdan büyük olmalıdır")
            .LessThanOrEqualTo(1000000).WithMessage("İşlem tutarı 1.000.000 TL'yi aşamaz");

        RuleFor(x => x.Dto.Currency)
            .NotEmpty().WithMessage("Para birimi boş olamaz")
            .Length(3).WithMessage("Para birimi 3 karakter olmalıdır");

        RuleFor(x => x.Dto.CardNumberMasked)
            .NotEmpty().WithMessage("Kart numarası boş olamaz")
            .MaximumLength(25).WithMessage("Kart numarası geçersiz");

        RuleFor(x => x.Dto.CardNumberEncrypted)
            .NotEmpty().WithMessage("Şifreli kart numarası boş olamaz");

        RuleFor(x => x.Dto.MerchantId)
            .NotEmpty().WithMessage("Üye işyeri ID boş olamaz");

        RuleFor(x => x.Dto.MerchantCode)
            .NotEmpty().WithMessage("Üye işyeri kodu boş olamaz")
            .MaximumLength(15).WithMessage("Üye işyeri kodu geçersiz");

        RuleFor(x => x.Dto.TerminalId)
            .NotEmpty().WithMessage("Terminal ID boş olamaz");

        RuleFor(x => x.Dto.TerminalCode)
            .NotEmpty().WithMessage("Terminal kodu boş olamaz")
            .MaximumLength(8).WithMessage("Terminal kodu geçersiz");

        RuleFor(x => x.Dto.OriginalTransactionId)
            .NotEmpty()
            .When(x => x.Dto.TransactionTypeId == 2 || x.Dto.TransactionTypeId == 3)
            .WithMessage("İade/İptal işlemleri için orijinal işlem ID gerekli");
    }
}