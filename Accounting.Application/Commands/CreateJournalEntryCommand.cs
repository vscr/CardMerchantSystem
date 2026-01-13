using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record CreateJournalEntryCommand(CreateJournalEntryDto Dto) : IRequest<Result<JournalEntryDto>>;