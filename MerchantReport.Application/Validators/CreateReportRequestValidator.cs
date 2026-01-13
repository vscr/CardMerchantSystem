using MerchantReport.Application.DTOs;
using FluentValidation;

namespace MerchantReport.Application.Validators;

public class CreateReportRequestValidator : AbstractValidator<CreateReportRequestDto>
{
    public CreateReportRequestValidator()
    {
        RuleFor(x => x.MerchantId)
            .NotEmpty().WithMessage("Üye işyeri ID boş olamaz")
            .MaximumLength(50).WithMessage("Üye işyeri ID en fazla 50 karakter olabilir");

        RuleFor(x => x.MerchantName)
            .NotEmpty().WithMessage("Üye işyeri adı boş olamaz")
            .MaximumLength(200).WithMessage("Üye işyeri adı en fazla 200 karakter olabilir");

        RuleFor(x => x.ReportTypeId)
            .GreaterThan(0).WithMessage("Geçerli bir rapor tipi seçiniz");

        RuleFor(x => x.ReportFormatId)
            .GreaterThan(0).WithMessage("Geçerli bir rapor formatı seçiniz");

        RuleFor(x => x.DeliveryMethodId)
            .GreaterThan(0).WithMessage("Geçerli bir dağıtım yöntemi seçiniz");

        RuleFor(x => x.PeriodStart)
            .NotEmpty().WithMessage("Dönem başlangıç tarihi boş olamaz");

        RuleFor(x => x.PeriodEnd)
            .NotEmpty().WithMessage("Dönem bitiş tarihi boş olamaz")
            .GreaterThanOrEqualTo(x => x.PeriodStart).WithMessage("Dönem bitiş tarihi başlangıçtan küçük olamaz");
    }
}