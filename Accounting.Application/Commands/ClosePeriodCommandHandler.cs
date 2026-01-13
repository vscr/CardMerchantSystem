using Accounting.Application.DTOs;
using Accounting.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public class ClosePeriodCommandHandler
    : IRequestHandler<ClosePeriodCommand, Result<AccountingPeriodDto>>
{
    private readonly IAccountingPeriodRepository _repository;

    public ClosePeriodCommandHandler(IAccountingPeriodRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AccountingPeriodDto>> Handle(
        ClosePeriodCommand request,
        CancellationToken cancellationToken)
    {
        var period = await _repository.GetByCodeAsync(request.PeriodCode, cancellationToken);
        if (period == null)
            return Result.Failure<AccountingPeriodDto>("Dönem bulunamadı");

        // Kapatma işlemini başlat
        var startResult = period.StartClosing();
        if (startResult.IsFailure)
            return Result.Failure<AccountingPeriodDto>(startResult.Error!);

        // Dönemi kapat
        var closeResult = period.Close(request.ClosedBy);
        if (closeResult.IsFailure)
            return Result.Failure<AccountingPeriodDto>(closeResult.Error!);

        await _repository.UpdateAsync(period, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

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