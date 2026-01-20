using Accounting.Application.DTOs;
using Accounting.Domain.Repositories;
using Accounting.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record ReverseJournalEntryCommand(Guid JournalEntryId, string ReversedBy) : IRequest<Result<JournalEntryDto>>;
public class ReverseJournalEntryCommandHandler
    : IRequestHandler<ReverseJournalEntryCommand, Result<JournalEntryDto>>
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IAccountingPeriodRepository _periodRepository;
    private readonly ITrialBalanceService _trialBalanceService;

    public ReverseJournalEntryCommandHandler(
        IJournalEntryRepository journalRepository,
        IAccountingPeriodRepository periodRepository,
        ITrialBalanceService trialBalanceService)
    {
        _journalRepository = journalRepository;
        _periodRepository = periodRepository;
        _trialBalanceService = trialBalanceService;
    }

    public async Task<Result<JournalEntryDto>> Handle(
        ReverseJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var entry = await _journalRepository.GetByIdWithLinesAsync(request.JournalEntryId, cancellationToken);
        if (entry == null)
            return Result.Failure<JournalEntryDto>("Muhasebe fişi bulunamadı");

        // Güncel dönem
        var currentPeriod = await _periodRepository.GetCurrentPeriodAsync(cancellationToken);
        if (currentPeriod == null)
            return Result.Failure<JournalEntryDto>("Açık dönem bulunamadı");

        // Ters kayıt oluştur
        var reverseResult = entry.Reverse(currentPeriod.PeriodCode, request.ReversedBy);
        if (reverseResult.IsFailure)
            return Result.Failure<JournalEntryDto>(reverseResult.Error!);

        var reverseEntry = reverseResult.Value!;

        // Hesap bakiyelerini güncelle (ters kayıt için)
        foreach (var line in reverseEntry.Lines)
        {
            await _trialBalanceService.UpdateAccountBalanceAsync(
                line.AccountId,
                reverseEntry.PeriodCode,
                line.DebitAmount,
                line.CreditAmount,
                cancellationToken);
        }

        await _journalRepository.AddAsync(reverseEntry, cancellationToken);
        await _journalRepository.UpdateAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(reverseEntry);
    }

    private static JournalEntryDto MapToDto(Domain.Entities.JournalEntry entry)
    {
        return new JournalEntryDto
        {
            Id = entry.Id,
            EntryNumber = entry.EntryNumber,
            EntryDate = entry.EntryDate,
            PeriodCode = entry.PeriodCode,
            TransactionType = entry.TransactionType.Name,
            TransactionTypeDisplayName = entry.TransactionType.DisplayName,
            Status = entry.Status.Name,
            StatusDisplayName = entry.Status.DisplayName,
            Description = entry.Description,
            ReferenceNumber = entry.ReferenceNumber,
            ReferenceType = entry.ReferenceType,
            ReferenceId = entry.ReferenceId,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsBalanced = entry.IsBalanced,
            PostedAt = entry.PostedAt,
            PostedBy = entry.PostedBy,
            CreatedAt = entry.CreatedAt,
            Lines = entry.Lines.Select(l => new JournalEntryLineDto
            {
                Id = l.Id,
                LineNumber = l.LineNumber,
                AccountId = l.AccountId,
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                DebitAmount = l.DebitAmount,
                CreditAmount = l.CreditAmount,
                NetAmount = l.NetAmount,
                Description = l.Description
            }).ToList()
        };
    }
}