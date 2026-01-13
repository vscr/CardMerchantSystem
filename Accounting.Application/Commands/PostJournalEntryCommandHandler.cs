using Accounting.Application.DTOs;
using Accounting.Domain.Repositories;
using Accounting.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public class PostJournalEntryCommandHandler
    : IRequestHandler<PostJournalEntryCommand, Result<JournalEntryDto>>
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly ITrialBalanceService _trialBalanceService;

    public PostJournalEntryCommandHandler(
        IJournalEntryRepository journalRepository,
        ITrialBalanceService trialBalanceService)
    {
        _journalRepository = journalRepository;
        _trialBalanceService = trialBalanceService;
    }

    public async Task<Result<JournalEntryDto>> Handle(
        PostJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var entry = await _journalRepository.GetByIdWithLinesAsync(request.JournalEntryId, cancellationToken);
        if (entry == null)
            return Result.Failure<JournalEntryDto>("Muhasebe fişi bulunamadı");

        // Fişi onayla
        var postResult = entry.Post(request.PostedBy);
        if (postResult.IsFailure)
            return Result.Failure<JournalEntryDto>(postResult.Error!);

        // Hesap bakiyelerini güncelle
        foreach (var line in entry.Lines)
        {
            await _trialBalanceService.UpdateAccountBalanceAsync(
                line.AccountId,
                entry.PeriodCode,
                line.DebitAmount,
                line.CreditAmount,
                cancellationToken);
        }

        await _journalRepository.UpdateAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(entry);
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