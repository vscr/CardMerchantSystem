using BKM.Application.DTOs;
using BKM.Domain.Entities;
using BKM.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Queries;

public class GetSwitchMessageByIdQueryHandler
    : IRequestHandler<GetSwitchMessageByIdQuery, Result<SwitchMessageDto>>
{
    private readonly ISwitchMessageRepository _repository;

    public GetSwitchMessageByIdQueryHandler(ISwitchMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SwitchMessageDto>> Handle(
        GetSwitchMessageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var message = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (message == null)
            return Result.Failure<SwitchMessageDto>("Mesaj bulunamadı", ErrorCodes.NotFound);

        return MapToDto(message);
    }

    private static SwitchMessageDto MapToDto(SwitchMessage m)
    {
        return new SwitchMessageDto
        {
            Id = m.Id,
            MessageType = m.MessageType.Name,
            MessageTypeDisplayName = m.MessageType.DisplayName,
            ProcessingCode = m.ProcessingCode.Name,
            ProcessingCodeDisplayName = m.ProcessingCode.DisplayName,
            Status = m.Status.Name,
            StatusDisplayName = m.Status.DisplayName,
            STAN = m.STAN,
            RRN = m.RRN,
            CardNumberMasked = m.CardNumberMasked,
            BIN = m.BIN,
            Amount = m.Amount,
            Currency = m.Currency,
            TransactionDateTime = m.TransactionDateTime,
            TerminalId = m.TerminalId,
            MerchantId = m.MerchantId,
            MCC = m.MCC,
            AcquirerBankCode = m.AcquirerBankCode,
            IssuerBankCode = m.IssuerBankCode,
            ResponseCode = m.ResponseCode?.Name,
            ResponseCodeDisplayName = m.ResponseCode?.DisplayName,
            AuthorizationCode = m.AuthorizationCode,
            ErrorMessage = m.ErrorMessage,
            ReceivedAt = m.ReceivedAt,
            ProcessedAt = m.ProcessedAt,
            RespondedAt = m.RespondedAt,
            ProcessingTimeMs = m.ProcessingTimeMs
        };
    }
}