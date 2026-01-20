using BKM.Application.DTOs;
using BKM.Domain.Entities;
using BKM.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Commands;

public record ProcessSettlementCommand(string SettlementDate) : IRequest<Result<SettlementBatchDto>>;
public class ProcessSettlementCommandHandler
    : IRequestHandler<ProcessSettlementCommand, Result<SettlementBatchDto>>
{
    private readonly ISettlementRepository _settlementRepository;
    private readonly IClearingRepository _clearingRepository;

    public ProcessSettlementCommandHandler(
        ISettlementRepository settlementRepository,
        IClearingRepository clearingRepository)
    {
        _settlementRepository = settlementRepository;
        _clearingRepository = clearingRepository;
    }

    public async Task<Result<SettlementBatchDto>> Handle(
        ProcessSettlementCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Settlement batch oluştur
        var batchResult = SettlementBatch.Create(request.SettlementDate);
        if (batchResult.IsFailure)
            return Result.Failure<SettlementBatchDto>(batchResult.Error!);

        var batch = batchResult.Value!;

        // 2. Unsettled clearing kayıtlarını al
        var unsettledRecords = await _clearingRepository.GetUnsettledAsync(cancellationToken);

        if (!unsettledRecords.Any())
            return Result.Failure<SettlementBatchDto>("Settle edilecek kayıt bulunamadı");

        // 3. Kayıtları batch'e ekle
        foreach (var record in unsettledRecords)
        {
            batch.AddClearingRecord(record);
            await _clearingRepository.UpdateAsync(record, cancellationToken);
        }

        // 4. Batch'i tamamla
        var completeResult = batch.Complete();
        if (completeResult.IsFailure)
            return Result.Failure<SettlementBatchDto>(completeResult.Error!);

        // 5. Kaydet
        await _settlementRepository.AddAsync(batch, cancellationToken);
        await _settlementRepository.SaveChangesAsync(cancellationToken);
        await _clearingRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(batch);
    }

    private static SettlementBatchDto MapToDto(SettlementBatch batch)
    {
        return new SettlementBatchDto
        {
            Id = batch.Id,
            SettlementDate = batch.SettlementDate,
            BatchNumber = batch.BatchNumber,
            TotalTransactionCount = batch.TotalTransactionCount,
            TotalTransactionAmount = batch.TotalTransactionAmount,
            TotalFeeAmount = batch.TotalFeeAmount,
            TotalNetAmount = batch.TotalNetAmount,
            IsCompleted = batch.IsCompleted,
            CompletedAt = batch.CompletedAt,
            CreatedAt = batch.CreatedAt,
            BankSummaries = batch.BankSummaries.Select(s => new BankSettlementSummaryDto
            {
                Id = s.Id,
                BankCode = s.BankCode,
                IsAcquirer = s.IsAcquirer,
                TransactionCount = s.TransactionCount,
                TotalAmount = s.TotalAmount,
                TotalFee = s.TotalFee,
                NetAmount = s.NetAmount
            }).ToList()
        };
    }
}