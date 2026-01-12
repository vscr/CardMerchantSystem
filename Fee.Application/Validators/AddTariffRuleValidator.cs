using Fee.Application.DTOs;
using FluentValidation;

namespace Fee.Application.Validators;

public class AddTariffRuleValidator : AbstractValidator<AddTariffRuleDto>
{
    public AddTariffRuleValidator()
    {
        RuleFor(x => x.TariffId)
            .NotEmpty().WithMessage("Tarife ID boş olamaz");

        RuleFor(x => x.CalculationTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir hesaplama tipi seçiniz");

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Oran negatif olamaz")
            .LessThanOrEqualTo(100).WithMessage("Oran %100'den büyük olamaz");

        RuleFor(x => x.MinimumFee)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinimumFee.HasValue)
            .WithMessage("Minimum ücret negatif olamaz");

        RuleFor(x => x.MaximumFee)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaximumFee.HasValue)
            .WithMessage("Maksimum ücret negatif olamaz");

        RuleFor(x => x.MaximumFee)
            .GreaterThanOrEqualTo(x => x.MinimumFee)
            .When(x => x.MinimumFee.HasValue && x.MaximumFee.HasValue)
            .WithMessage("Maksimum ücret minimum ücretten küçük olamaz");

        RuleFor(x => x.MCC)
            .Length(4)
            .When(x => !string.IsNullOrEmpty(x.MCC))
            .WithMessage("MCC 4 karakter olmalı");

        RuleFor(x => x.InstallmentCount)
            .InclusiveBetween(1, 36)
            .When(x => x.InstallmentCount.HasValue)
            .WithMessage("Taksit sayısı 1-36 arasında olmalı");

        RuleFor(x => x.VolumeTo)
            .GreaterThan(x => x.VolumeFrom)
            .When(x => x.VolumeFrom.HasValue && x.VolumeTo.HasValue)
            .WithMessage("Hacim bitiş değeri başlangıçtan büyük olmalı");
    }
}