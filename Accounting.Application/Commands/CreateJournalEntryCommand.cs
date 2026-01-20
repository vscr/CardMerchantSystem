using Accounting.Application.DTOs;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record CreateJournalEntryCommand(CreateJournalEntryDto Dto) : IRequest<Result<JournalEntryDto>>;

public class CreateJournalEntryCommandHandler
    : IRequestHandler<CreateJournalEntryCommand, Result<JournalEntryDto>>
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IAccountingPeriodRepository _periodRepository;
    private readonly IChartOfAccountRepository _accountRepository;

    public CreateJournalEntryCommandHandler(
        IJournalEntryRepository journalRepository,
        IAccountingPeriodRepository periodRepository,
        IChartOfAccountRepository accountRepository)
    {
        _journalRepository = journalRepository;
        _periodRepository = periodRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Result<JournalEntryDto>> Handle(
        CreateJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Dönem kontrolü
        var periodCode = dto.EntryDate.ToString("yyyyMM");
        var period = await _periodRepository.GetByCodeAsync(periodCode, cancellationToken);
        if (period == null)
            return Result.Failure<JournalEntryDto>($"Dönem bulunamadı: {periodCode}");

        if (!period.Status.AllowsPosting)
            return Result.Failure<JournalEntryDto>("Dönem kayda kapalı");

        var transactionType = TransactionType.FromId<TransactionType>(dto.TransactionTypeId);
        if (transactionType == null)
            return Result.Failure<JournalEntryDto>("Geçersiz işlem tipi");

        // Fiş oluştur
        var entryResult = JournalEntry.Create(
            dto.EntryDate,
            periodCode,
            transactionType,
            dto.Description,
            dto.ReferenceNumber,
            dto.ReferenceType,
            dto.ReferenceId);

        if (entryResult.IsFailure)
            return Result.Failure<JournalEntryDto>(entryResult.Error!);

        var entry = entryResult.Value!;

        // Satırları ekle
        foreach (var lineDto in dto.Lines)
        {
            var account = await _accountRepository.GetByIdAsync(lineDto.AccountId, cancellationToken);
            if (account == null)
                return Result.Failure<JournalEntryDto>($"Hesap bulunamadı: {lineDto.AccountId}");

            if (!account.IsPostable)
                return Result.Failure<JournalEntryDto>($"Bu hesaba kayıt yapılamaz: {account.AccountCode}");

            var lineResult = entry.AddLine(
                account.Id,
                account.AccountCode,
                account.AccountName,
                lineDto.DebitAmount,
                lineDto.CreditAmount,
                lineDto.Description);

            if (lineResult.IsFailure)
                return Result.Failure<JournalEntryDto>(lineResult.Error!);
        }

        await _journalRepository.AddAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(entry);
    }

    private static JournalEntryDto MapToDto(JournalEntry entry)
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