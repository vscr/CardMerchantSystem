using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record ReverseJournalEntryCommand(Guid JournalEntryId, string ReversedBy) : IRequest<Result<JournalEntryDto>>;