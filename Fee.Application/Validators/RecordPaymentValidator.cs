using Fee.Application.DTOs;
using FluentValidation;

namespace Fee.Application.Validators;

public class RecordPaymentValidator : AbstractValidator<RecordPaymentDto>
{
    public RecordPaymentValidator()
    {
        RuleFor(x => x.AccrualId)
            .NotEmpty().WithMessage("Tahakkuk ID boş olamaz");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Ödeme tutarı sıfırdan büyük olmalı");
    }
}