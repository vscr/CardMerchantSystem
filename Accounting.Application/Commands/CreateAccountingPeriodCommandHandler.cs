using Accounting.Application.DTOs;
using Accounting.Domain.Entities;
using Accounting.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public class CreateAccountingPeriodCommandHandler
    : IRequestHandler<CreateAccountingPeriodCommand, Result<AccountingPeriodDto>>
{
    private readonly IAccountingPeriodRepository _repository;

    public CreateAccountingPeriodCommandHandler(IAccountingPeriodRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AccountingPeriodDto>> Handle(
        CreateAccountingPeriodCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var periodCode = $"{dto.Year}{dto.Month:D2}";

        // Aynı dönem var mı?
        var existing = await _repository.GetByCodeAsync(periodCode, cancellationToken);
        if (existing != null)
            return Result.Failure<AccountingPeriodDto>("Bu dönem zaten mevcut");

        var periodResult = AccountingPeriod.Create(dto.Year, dto.Month);
        if (periodResult.IsFailure)
            return Result.Failure<AccountingPeriodDto>(periodResult.Error!);

        var period = periodResult.Value!;

        await _repository.AddAsync(period, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(period);
    }

    private static AccountingPeriodDto MapToDto(AccountingPeriod period)
    {
        return new AccountingPeriodDto
        {
            Id = period.Id,
            PeriodCode = period.PeriodCode,
            PeriodName = period.PeriodName,
            Year = period.Year,
            Month = period.Month,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            Status = period.Status.Name,
            StatusDisplayName = period.Status.DisplayName,
            ClosedAt = period.ClosedAt,
            ClosedBy = period.ClosedBy,
            CreatedAt = period.CreatedAt
        };
    }
}