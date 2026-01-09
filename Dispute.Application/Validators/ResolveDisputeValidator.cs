using Dispute.Application.Commands;
using FluentValidation;

namespace Dispute.Application.Validators;

public class ResolveDisputeValidator : AbstractValidator<ResolveDisputeCommand>
{
    public ResolveDisputeValidator()
    {
        RuleFor(x => x.DisputeId)
            .NotEmpty().WithMessage("İtiraz ID boş olamaz");

        RuleFor(x => x.Resolution)
            .NotEmpty().WithMessage("Çözüm açıklaması boş olamaz")
            .MinimumLength(10).WithMessage("Çözüm açıklaması en az 10 karakter olmalıdır")
            .MaximumLength(500).WithMessage("Çözüm açıklaması en fazla 500 karakter olabilir");

        RuleFor(x => x.OperatorUsername)
            .NotEmpty().WithMessage("Operatör kullanıcı adı boş olamaz");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0).When(x => x.InFavorOfCustomer)
            .WithMessage("Müşteri lehine çözümde iade tutarı belirtilmeli");
    }
}