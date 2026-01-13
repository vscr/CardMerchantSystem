using Accounting.Application.DTOs;
using Accounting.Domain.Entities;
using Accounting.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Queries;

public class GetJournalEntryByIdQueryHandler
    : IRequestHandler<GetJournalEntryByIdQuery, Result<JournalEntryDto>>
{
    private readonly IJournalEntryRepository _repository;

    public GetJournalEntryByIdQueryHandler(IJournalEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<JournalEntryDto>> Handle(
        GetJournalEntryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entry = await _repository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (entry == null)
            return Result.Failure<JournalEntryDto>("Muhasebe fişi bulunamadı", ErrorCodes.NotFound);

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