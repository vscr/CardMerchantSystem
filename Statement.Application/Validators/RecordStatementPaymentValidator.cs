using Statement.Application.DTOs;
using FluentValidation;

namespace Statement.Application.Validators;

public class RecordStatementPaymentValidator : AbstractValidator<RecordStatementPaymentDto>
{
    public RecordStatementPaymentValidator()
    {
        RuleFor(x => x.StatementId)
            .NotEmpty().WithMessage("Ekstre ID boş olamaz");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Ödeme tutarı sıfırdan büyük olmalı");
    }
}