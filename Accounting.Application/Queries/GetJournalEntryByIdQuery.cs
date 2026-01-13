using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Queries;

public record GetJournalEntryByIdQuery(Guid Id) : IRequest<Result<JournalEntryDto>>;