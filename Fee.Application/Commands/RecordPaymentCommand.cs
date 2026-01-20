using CardMerchantSystem.Shared.Kernel;
using Fee.Application.DTOs;
using Fee.Domain.Entities;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Commands;

public record RecordPaymentCommand(RecordPaymentDto Dto) : IRequest<Result<FeeAccrualDto>>;
public class RecordPaymentCommandHandler
    : IRequestHandler<RecordPaymentCommand, Result<FeeAccrualDto>>
{
    private readonly IFeeAccrualRepository _repository;

    public RecordPaymentCommandHandler(IFeeAccrualRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<FeeAccrualDto>> Handle(
        RecordPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var accrual = await _repository.GetByIdAsync(dto.AccrualId, cancellationToken);
        if (accrual == null)
            return Result.Failure<FeeAccrualDto>("Tahakkuk bulunamadı");

        var paymentResult = accrual.RecordPayment(dto.Amount);
        if (paymentResult.IsFailure)
            return Result.Failure<FeeAccrualDto>(paymentResult.Error!);

        await _repository.UpdateAsync(accrual, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(accrual);
    }

    private static FeeAccrualDto MapToDto(FeeAccrual accrual)
    {
        return new FeeAccrualDto
        {
            Id = accrual.Id,
            AccrualNumber = accrual.AccrualNumber,
            FeeType = accrual.FeeType.Name,
            FeeTypeDisplayName = accrual.FeeType.DisplayName,
            Status = accrual.Status.Name,
            StatusDisplayName = accrual.Status.DisplayName,
            Period = accrual.Period.Name,
            PeriodDisplayName = accrual.Period.DisplayName,
            MerchantId = accrual.MerchantId,
            CardNumber = accrual.CardNumber,
            TerminalId = accrual.TerminalId,
            GrossAmount = accrual.GrossAmount,
            DiscountAmount = accrual.DiscountAmount,
            NetAmount = accrual.NetAmount,
            PaidAmount = accrual.PaidAmount,
            RemainingAmount = accrual.RemainingAmount,
            AccrualDate = accrual.AccrualDate,
            DueDate = accrual.DueDate,
            PaidDate = accrual.PaidDate,
            AccrualPeriodStart = accrual.AccrualPeriodStart,
            AccrualPeriodEnd = accrual.AccrualPeriodEnd
        };
    }
}