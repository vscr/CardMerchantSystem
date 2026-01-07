using Card.Application.Commands;
using FluentValidation;

namespace Card.Application.Validators;

public class RejectCardApplicationValidator : AbstractValidator<RejectCardApplicationCommand>
{
    public RejectCardApplicationValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Başvuru ID boş olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Red nedeni boş olamaz")
            .MinimumLength(10).WithMessage("Red nedeni en az 10 karakter olmalıdır")
            .MaximumLength(500).WithMessage("Red nedeni en fazla 500 karakter olabilir");

        RuleFor(x => x.RejectorUsername)
            .NotEmpty().WithMessage("Reddeden kullanıcı adı boş olamaz")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir");
    }
}