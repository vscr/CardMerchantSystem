using Dispute.Application.Commands;
using FluentValidation;

namespace Dispute.Application.Validators;

public class CreateDisputeValidator : AbstractValidator<CreateDisputeCommand>
{
    public CreateDisputeValidator()
    {
        RuleFor(x => x.Dto.TransactionId)
            .NotEmpty().WithMessage("İşlem ID boş olamaz");

        RuleFor(x => x.Dto.TransactionReference)
            .NotEmpty().WithMessage("İşlem referans numarası boş olamaz")
            .MaximumLength(50).WithMessage("İşlem referans numarası en fazla 50 karakter olabilir");

        RuleFor(x => x.Dto.TransactionAmount)
            .GreaterThan(0).WithMessage("İşlem tutarı sıfırdan büyük olmalıdır");

        RuleFor(x => x.Dto.DisputedAmount)
            .GreaterThan(0).WithMessage("İtiraz tutarı sıfırdan büyük olmalıdır")
            .LessThanOrEqualTo(x => x.Dto.TransactionAmount)
            .WithMessage("İtiraz tutarı işlem tutarından büyük olamaz");

        RuleFor(x => x.Dto.ReasonId)
            .InclusiveBetween(1, 9).WithMessage("Geçersiz itiraz nedeni");

        RuleFor(x => x.Dto.Description)
            .NotEmpty().WithMessage("İtiraz açıklaması boş olamaz")
            .MinimumLength(10).WithMessage("İtiraz açıklaması en az 10 karakter olmalıdır")
            .MaximumLength(1000).WithMessage("İtiraz açıklaması en fazla 1000 karakter olabilir");

        RuleFor(x => x.Dto.CustomerTckn)
            .NotEmpty().WithMessage("Müşteri TCKN boş olamaz")
            .Length(11).WithMessage("Müşteri TCKN 11 haneli olmalıdır");

        RuleFor(x => x.Dto.CustomerName)
            .NotEmpty().WithMessage("Müşteri adı boş olamaz")
            .MaximumLength(100).WithMessage("Müşteri adı en fazla 100 karakter olabilir");

        RuleFor(x => x.Dto.CustomerPhone)
            .NotEmpty().WithMessage("Müşteri telefonu boş olamaz")
            .Matches(@"^[0-9]{10,11}$").WithMessage("Geçerli bir telefon numarası giriniz");

        RuleFor(x => x.Dto.CustomerEmail)
            .NotEmpty().WithMessage("Müşteri e-posta adresi boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

        RuleFor(x => x.Dto.MerchantId)
            .NotEmpty().WithMessage("Üye işyeri ID boş olamaz");

        RuleFor(x => x.Dto.MerchantCode)
            .NotEmpty().WithMessage("Üye işyeri kodu boş olamaz")
            .MaximumLength(15).WithMessage("Üye işyeri kodu en fazla 15 karakter olabilir");

        RuleFor(x => x.Dto.MerchantName)
            .NotEmpty().WithMessage("Üye işyeri adı boş olamaz")
            .MaximumLength(100).WithMessage("Üye işyeri adı en fazla 100 karakter olabilir");
    }
}