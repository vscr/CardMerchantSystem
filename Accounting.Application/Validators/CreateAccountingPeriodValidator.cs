using Accounting.Application.DTOs;
using FluentValidation;

namespace Accounting.Application.Validators;

public class CreateAccountingPeriodValidator : AbstractValidator<CreateAccountingPeriodDto>
{
    public CreateAccountingPeriodValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100).WithMessage("Yıl 2000-2100 arasında olmalı");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("Ay 1-12 arasında olmalı");
    }
}