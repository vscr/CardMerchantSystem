using Transaction.Application.Commands;
using FluentValidation;

namespace Transaction.Application.Validators;

public class RefundTransactionValidator : AbstractValidator<RefundTransactionCommand>
{
    public RefundTransactionValidator()
    {
        RuleFor(x => x.OriginalTransactionId)
            .NotEmpty().WithMessage("Orijinal işlem ID boş olamaz");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("İade tutarı sıfırdan büyük olmalıdır");

        RuleFor(x => x.OperatorUsername)
            .NotEmpty().WithMessage("Operatör kullanıcı adı boş olamaz")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir");
    }
}