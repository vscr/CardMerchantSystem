using Card.Application.Commands;
using FluentValidation;

namespace Card.Application.Validators;

public class CreateCardApplicationValidator : AbstractValidator<CreateCardApplicationCommand>
{
    public CreateCardApplicationValidator()
    {
        RuleFor(x => x.Dto.CustomerTckn)
            .NotEmpty().WithMessage("TCKN boş olamaz")
            .Length(11).WithMessage("TCKN 11 haneli olmalıdır")
            .Matches("^[0-9]+$").WithMessage("TCKN sadece rakamlardan oluşmalıdır")
            .Must(x => x == null || x[0] != '0').WithMessage("TCKN 0 ile başlayamaz");

        RuleFor(x => x.Dto.CustomerName)
            .NotEmpty().WithMessage("Müşteri adı boş olamaz")
            .MinimumLength(2).WithMessage("Müşteri adı en az 2 karakter olmalıdır")
            .MaximumLength(50).WithMessage("Müşteri adı en fazla 50 karakter olabilir");

        RuleFor(x => x.Dto.CustomerSurname)
            .NotEmpty().WithMessage("Müşteri soyadı boş olamaz")
            .MinimumLength(2).WithMessage("Müşteri soyadı en az 2 karakter olmalıdır")
            .MaximumLength(50).WithMessage("Müşteri soyadı en fazla 50 karakter olabilir");

        RuleFor(x => x.Dto.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz")
            .Matches(@"^[0-9]{10,11}$").WithMessage("Geçerli bir telefon numarası giriniz");

        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

        RuleFor(x => x.Dto.Street)
            .NotEmpty().WithMessage("Sokak/Cadde boş olamaz")
            .MaximumLength(200).WithMessage("Sokak/Cadde en fazla 200 karakter olabilir");

        RuleFor(x => x.Dto.District)
            .NotEmpty().WithMessage("İlçe boş olamaz")
            .MaximumLength(50).WithMessage("İlçe en fazla 50 karakter olabilir");

        RuleFor(x => x.Dto.City)
            .NotEmpty().WithMessage("Şehir boş olamaz")
            .MaximumLength(50).WithMessage("Şehir en fazla 50 karakter olabilir");

        RuleFor(x => x.Dto.PostalCode)
            .NotEmpty().WithMessage("Posta kodu boş olamaz")
            .Matches(@"^[0-9]{5}$").WithMessage("Posta kodu 5 haneli olmalıdır");

        RuleFor(x => x.Dto.CardTypeId)
            .InclusiveBetween(1, 5).WithMessage("Geçersiz kart tipi");

        RuleFor(x => x.Dto.DailyLimit)
            .GreaterThan(0).When(x => x.Dto.DailyLimit.HasValue)
            .WithMessage("Günlük limit 0'dan büyük olmalıdır");

        RuleFor(x => x.Dto.MonthlyLimit)
            .GreaterThan(0).When(x => x.Dto.MonthlyLimit.HasValue)
            .WithMessage("Aylık limit 0'dan büyük olmalıdır");

        RuleFor(x => x.Dto)
            .Must(x => !x.DailyLimit.HasValue || !x.MonthlyLimit.HasValue || x.DailyLimit <= x.MonthlyLimit)
            .WithMessage("Günlük limit aylık limitten büyük olamaz");
    }
}