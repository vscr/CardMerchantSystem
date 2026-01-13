using MerchantReport.Application.DTOs;
using FluentValidation;

namespace MerchantReport.Application.Validators;

public class SetFtpDeliveryValidator : AbstractValidator<SetFtpDeliveryDto>
{
    public SetFtpDeliveryValidator()
    {
        RuleFor(x => x.ConfigId)
            .NotEmpty().WithMessage("Ayar ID boş olamaz");

        RuleFor(x => x.Host)
            .NotEmpty().WithMessage("FTP sunucu adresi boş olamaz")
            .MaximumLength(200).WithMessage("FTP sunucu adresi en fazla 200 karakter olabilir");

        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535).WithMessage("Port 1-65535 arasında olmalı");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
            .MaximumLength(100).WithMessage("Kullanıcı adı en fazla 100 karakter olabilir");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz")
            .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("FTP dizini boş olamaz")
            .MaximumLength(200).WithMessage("FTP dizini en fazla 200 karakter olabilir");
    }
}