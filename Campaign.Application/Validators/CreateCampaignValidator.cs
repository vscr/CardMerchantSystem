using Campaign.Application.Commands;
using FluentValidation;

namespace Campaign.Application.Validators;

public class CreateCampaignValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Kampanya adı boş olamaz")
            .MaximumLength(100).WithMessage("Kampanya adı en fazla 100 karakter olabilir");

        RuleFor(x => x.Dto.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir");

        RuleFor(x => x.Dto.CampaignTypeId)
            .InclusiveBetween(1, 6).WithMessage("Geçersiz kampanya tipi");

        RuleFor(x => x.Dto.DiscountTypeId)
            .InclusiveBetween(1, 2).WithMessage("Geçersiz indirim tipi");

        RuleFor(x => x.Dto.TargetAudienceId)
            .InclusiveBetween(1, 6).WithMessage("Geçersiz hedef kitle");

        RuleFor(x => x.Dto.DiscountValue)
            .GreaterThan(0).WithMessage("İndirim değeri sıfırdan büyük olmalıdır");

        RuleFor(x => x.Dto.DiscountValue)
            .LessThanOrEqualTo(100)
            .When(x => x.Dto.DiscountTypeId == 1) // Percentage
            .WithMessage("Yüzde indirim 100'den büyük olamaz");

        RuleFor(x => x.Dto.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Başlangıç tarihi geçmiş olamaz");

        RuleFor(x => x.Dto.EndDate)
            .GreaterThan(x => x.Dto.StartDate)
            .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır");

        RuleFor(x => x.Dto.MaxDiscountAmount)
            .GreaterThan(0)
            .When(x => x.Dto.MaxDiscountAmount.HasValue)
            .WithMessage("Maksimum indirim tutarı sıfırdan büyük olmalıdır");

        RuleFor(x => x.Dto.MinTransactionAmount)
            .GreaterThan(0)
            .When(x => x.Dto.MinTransactionAmount.HasValue)
            .WithMessage("Minimum işlem tutarı sıfırdan büyük olmalıdır");

        RuleFor(x => x.Dto.TotalBudget)
            .GreaterThan(0)
            .When(x => x.Dto.TotalBudget.HasValue)
            .WithMessage("Toplam bütçe sıfırdan büyük olmalıdır");

        RuleFor(x => x.Dto.MaxUsageCount)
            .GreaterThan(0)
            .When(x => x.Dto.MaxUsageCount.HasValue)
            .WithMessage("Maksimum kullanım sayısı sıfırdan büyük olmalıdır");

        RuleFor(x => x.Dto.MaxUsagePerCustomer)
            .GreaterThan(0)
            .When(x => x.Dto.MaxUsagePerCustomer.HasValue)
            .WithMessage("Müşteri başına maksimum kullanım sıfırdan büyük olmalıdır");

        RuleFor(x => x.Dto.PointsMultiplier)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Puan çarpanı en az 1 olmalıdır");
    }
}