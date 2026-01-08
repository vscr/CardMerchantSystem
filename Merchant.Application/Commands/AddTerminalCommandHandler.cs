using Merchant.Application.DTOs;
using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

public class AddTerminalCommandHandler
    : IRequestHandler<AddTerminalCommand, Result<TerminalDto>>
{
    private readonly IMerchantRepository _repository;

    public AddTerminalCommandHandler(IMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TerminalDto>> Handle(
        AddTerminalCommand request,
        CancellationToken cancellationToken)
    {
        var merchant = await _repository.GetByIdWithTerminalsAsync(request.MerchantId, cancellationToken);

        if (merchant == null)
            return Result.Failure<TerminalDto>("Üye işyeri bulunamadı", ErrorCodes.MerchantNotFound);

        var terminalType = TerminalType.FromId<TerminalType>(request.Dto.TerminalTypeId);
        if (terminalType == null)
            return Result.Failure<TerminalDto>("Geçersiz terminal tipi", ErrorCodes.ValidationError);

        var result = merchant.AddTerminal(
            terminalType,
            request.Dto.SerialNumber,
            request.Dto.Model,
            request.Dto.Location);

        if (result.IsFailure)
            return Result.Failure<TerminalDto>(result.Error!, result.ErrorCode);

        await _repository.UpdateAsync(merchant, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(result.Value!);
    }

    private static TerminalDto MapToDto(Terminal t)
    {
        return new TerminalDto
        {
            Id = t.Id,
            MerchantId = t.MerchantId,
            TerminalCode = t.TerminalCode.Value,
            TerminalType = t.TerminalType.Name,
            Status = t.Status.Name,
            StatusDisplayName = t.Status.DisplayName,
            SerialNumber = t.SerialNumber,
            Model = t.Model,
            Location = t.Location,
            InstalledAt = t.InstalledAt,
            InstalledBy = t.InstalledBy,
            CreatedAt = t.CreatedAt
        };
    }
}