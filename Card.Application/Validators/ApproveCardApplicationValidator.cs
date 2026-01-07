using Card.Application.Commands;
using FluentValidation;

namespace Card.Application.Validators;

public class ApproveCardApplicationValidator : AbstractValidator<ApproveCardApplicationCommand>
{
    public ApproveCardApplicationValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Başvuru ID boş olamaz");

        RuleFor(x => x.ApproverUsername)
            .NotEmpty().WithMessage("Onaylayan kullanıcı adı boş olamaz")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir");
    }
}