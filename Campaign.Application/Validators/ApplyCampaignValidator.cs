using Campaign.Application.Commands;
using FluentValidation;

namespace Campaign.Application.Validators;

public class ApplyCampaignValidator : AbstractValidator<ApplyCampaignCommand>
{
    public ApplyCampaignValidator()
    {
        RuleFor(x => x.Dto.CampaignCode)
            .NotEmpty().WithMessage("Kampanya kodu boş olamaz")
            .MaximumLength(20).WithMessage("Kampanya kodu en fazla 20 karakter olabilir");

        RuleFor(x => x.Dto.TransactionId)
            .NotEmpty().WithMessage("İşlem ID boş olamaz");

        RuleFor(x => x.Dto.CardNumberMasked)
            .NotEmpty().WithMessage("Kart numarası boş olamaz");

        RuleFor(x => x.Dto.MerchantId)
            .NotEmpty().WithMessage("Üye işyeri ID boş olamaz");

        RuleFor(x => x.Dto.MerchantCode)
            .NotEmpty().WithMessage("Üye işyeri kodu boş olamaz");

        RuleFor(x => x.Dto.TransactionAmount)
            .GreaterThan(0).WithMessage("İşlem tutarı sıfırdan büyük olmalıdır");
    }
}