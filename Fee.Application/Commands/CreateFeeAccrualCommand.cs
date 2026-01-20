using CardMerchantSystem.Shared.Kernel;
using Fee.Application.DTOs;
using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Commands;

public record CreateFeeAccrualCommand(CreateFeeAccrualDto Dto) : IRequest<Result<FeeAccrualDto>>;
public class CreateFeeAccrualCommandHandler
    : IRequestHandler<CreateFeeAccrualCommand, Result<FeeAccrualDto>>
{
    private readonly IFeeAccrualRepository _repository;

    public CreateFeeAccrualCommandHandler(IFeeAccrualRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<FeeAccrualDto>> Handle(
        CreateFeeAccrualCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Fee type bul
        var feeType = FeeType.FromId<FeeType>(dto.FeeTypeId);
        if (feeType == null)
            return Result.Failure<FeeAccrualDto>("Geçersiz ücret tipi");

        // Period bul
        var period = AccrualPeriod.FromId<AccrualPeriod>(dto.PeriodId);
        if (period == null)
            return Result.Failure<FeeAccrualDto>("Geçersiz periyot");

        // Tahakkuk oluştur
        var accrualResult = FeeAccrual.Create(
            feeType,
            period,
            dto.GrossAmount,
            dto.DueDate,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.MerchantId,
            dto.CardNumber,
            dto.TerminalId,
            dto.DiscountAmount);

        if (accrualResult.IsFailure)
            return Result.Failure<FeeAccrualDto>(accrualResult.Error!);

        var accrual = accrualResult.Value!;

        await _repository.AddAsync(accrual, cancellationToken);
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