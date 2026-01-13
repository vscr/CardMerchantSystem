using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record PostJournalEntryCommand(Guid JournalEntryId, string PostedBy) : IRequest<Result<JournalEntryDto>>;