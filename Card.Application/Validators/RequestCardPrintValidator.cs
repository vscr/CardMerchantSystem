using Card.Application.Commands;
using FluentValidation;

namespace Card.Application.Validators;

public class RequestCardPrintValidator : AbstractValidator<RequestCardPrintCommand>
{
    public RequestCardPrintValidator()
    {
        RuleFor(x => x.ApplicationId)
            .NotEmpty().WithMessage("Başvuru ID boş olamaz");

        RuleFor(x => x.PrintVendorId)
            .InclusiveBetween(1, 3).WithMessage("Geçersiz basım firması");

        RuleFor(x => x.BatchId)
            .NotEmpty().WithMessage("Batch ID boş olamaz")
            .MaximumLength(50).WithMessage("Batch ID en fazla 50 karakter olabilir");

        RuleFor(x => x.OperatorUsername)
            .NotEmpty().WithMessage("Operatör kullanıcı adı boş olamaz")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir");
    }
}