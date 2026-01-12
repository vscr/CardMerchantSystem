using Fee.Application.DTOs;
using FluentValidation;

namespace Fee.Application.Validators;

public class CreateMembershipFeeValidator : AbstractValidator<CreateMembershipFeeDto>
{
    public CreateMembershipFeeValidator()
    {
        RuleFor(x => x.FeeName)
            .NotEmpty().WithMessage("Aidat adı boş olamaz")
            .MaximumLength(100).WithMessage("Aidat adı en fazla 100 karakter olabilir");

        RuleFor(x => x.FeeTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir ücret tipi seçiniz");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Tutar negatif olamaz");

        RuleFor(x => x.PeriodId)
            .GreaterThan(0).WithMessage("Geçerli bir periyot seçiniz");

        RuleFor(x => x.GracePeriodDays)
            .InclusiveBetween(0, 90).WithMessage("Ek süre 0-90 gün arasında olmalı");

        RuleFor(x => x.LateFeeRate)
            .InclusiveBetween(0, 50).WithMessage("Gecikme faizi %0-50 arasında olmalı");
    }
}