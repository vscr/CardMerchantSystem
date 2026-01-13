using MerchantReport.Application.DTOs;
using FluentValidation;

namespace MerchantReport.Application.Validators;

public class CreateMerchantReportConfigValidator : AbstractValidator<CreateMerchantReportConfigDto>
{
    public CreateMerchantReportConfigValidator()
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

        RuleFor(x => x.FrequencyId)
            .GreaterThan(0).WithMessage("Geçerli bir zamanlama sıklığı seçiniz");

        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(1, 7).WithMessage("Haftanın günü 1-7 arasında olmalı");

        RuleFor(x => x.DayOfMonth)
            .InclusiveBetween(1, 28).WithMessage("Ayın günü 1-28 arasında olmalı");

        RuleFor(x => x.RunTime)
            .NotEmpty().WithMessage("Çalışma saati boş olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Çalışma saati HH:mm formatında olmalı");
    }
}