using Merchant.Application.Commands;
using FluentValidation;

namespace Merchant.Application.Validators;

public class CreateMerchantValidator : AbstractValidator<CreateMerchantCommand>
{
    public CreateMerchantValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("İşyeri adı boş olamaz")
            .MinimumLength(2).WithMessage("İşyeri adı en az 2 karakter olmalıdır")
            .MaximumLength(100).WithMessage("İşyeri adı en fazla 100 karakter olabilir");

        RuleFor(x => x.Dto.TradeName)
            .NotEmpty().WithMessage("Ticari unvan boş olamaz")
            .MinimumLength(2).WithMessage("Ticari unvan en az 2 karakter olmalıdır")
            .MaximumLength(200).WithMessage("Ticari unvan en fazla 200 karakter olabilir");

        RuleFor(x => x.Dto.TaxNumber)
            .NotEmpty().WithMessage("Vergi numarası boş olamaz")
            .Length(10).WithMessage("Vergi numarası 10 haneli olmalıdır")
            .Matches("^[0-9]+$").WithMessage("Vergi numarası sadece rakamlardan oluşmalıdır");

        RuleFor(x => x.Dto.TaxOffice)
            .NotEmpty().WithMessage("Vergi dairesi boş olamaz")
            .MaximumLength(100).WithMessage("Vergi dairesi en fazla 100 karakter olabilir");

        RuleFor(x => x.Dto.MerchantTypeId)
            .InclusiveBetween(1, 5).WithMessage("Geçersiz üye işyeri tipi");

        RuleFor(x => x.Dto.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz")
            .Matches(@"^[0-9]{10,11}$").WithMessage("Geçerli bir telefon numarası giriniz");

        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

        RuleFor(x => x.Dto.Address)
            .NotEmpty().WithMessage("Adres boş olamaz")
            .MaximumLength(300).WithMessage("Adres en fazla 300 karakter olabilir");

        RuleFor(x => x.Dto.City)
            .NotEmpty().WithMessage("Şehir boş olamaz")
            .MaximumLength(50).WithMessage("Şehir en fazla 50 karakter olabilir");

        RuleFor(x => x.Dto.District)
            .NotEmpty().WithMessage("İlçe boş olamaz")
            .MaximumLength(50).WithMessage("İlçe en fazla 50 karakter olabilir");

        RuleFor(x => x.Dto.IBAN)
            .NotEmpty().WithMessage("IBAN boş olamaz")
            .Length(26).WithMessage("IBAN 26 karakter olmalıdır")
            .Matches("^TR[0-9]{24}$").WithMessage("Geçerli bir Türkiye IBAN'ı giriniz");

        RuleFor(x => x.Dto.CommissionRate)
            .InclusiveBetween(0, 100).WithMessage("Komisyon oranı 0-100 arasında olmalıdır");
    }
}