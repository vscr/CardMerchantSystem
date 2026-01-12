using Fee.Application.DTOs;
using Fee.Domain.Entities;
using Fee.Domain.Enums;
using Fee.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public class CreateMembershipFeeCommandHandler
    : IRequestHandler<CreateMembershipFeeCommand, Result<MembershipFeeDto>>
{
    private readonly IMembershipFeeRepository _repository;

    public CreateMembershipFeeCommandHandler(IMembershipFeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MembershipFeeDto>> Handle(
        CreateMembershipFeeCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Aynı isimde var mı?
        var existing = await _repository.GetByNameAsync(dto.FeeName, cancellationToken);
        if (existing != null)
            return Result.Failure<MembershipFeeDto>("Bu isimde aidat zaten mevcut");

        // Fee type bul
        var feeType = FeeType.FromId<FeeType>(dto.FeeTypeId);
        if (feeType == null)
            return Result.Failure<MembershipFeeDto>("Geçersiz ücret tipi");

        // Period bul
        var period = AccrualPeriod.FromId<AccrualPeriod>(dto.PeriodId);
        if (period == null)
            return Result.Failure<MembershipFeeDto>("Geçersiz periyot");

        // Aidat oluştur
        var feeResult = MembershipFee.Create(
            dto.FeeName,
            feeType,
            dto.Amount,
            period,
            dto.GracePeriodDays,
            dto.LateFeeRate,
            dto.Description,
            dto.MinimumTransactionVolume,
            dto.MinimumTransactionCount);

        if (feeResult.IsFailure)
            return Result.Failure<MembershipFeeDto>(feeResult.Error!);

        var fee = feeResult.Value!;

        await _repository.AddAsync(fee, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new MembershipFeeDto
        {
            Id = fee.Id,
            FeeName = fee.FeeName,
            FeeType = fee.FeeType.Name,
            FeeTypeDisplayName = fee.FeeType.DisplayName,
            Amount = fee.Amount,
            Period = fee.Period.Name,
            PeriodDisplayName = fee.Period.DisplayName,
            GracePeriodDays = fee.GracePeriodDays,
            LateFeeRate = fee.LateFeeRate,
            IsActive = fee.IsActive,
            Description = fee.Description,
            MinimumTransactionVolume = fee.MinimumTransactionVolume,
            MinimumTransactionCount = fee.MinimumTransactionCount,
            CreatedAt = fee.CreatedAt
        };
    }
}