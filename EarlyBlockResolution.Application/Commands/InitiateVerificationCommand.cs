using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Commands;

public record InitiateVerificationCommand(
    Guid CardBlockId,
    InitiateVerificationDto Dto,
    string OperatorUsername) : IRequest<Result<BlockVerificationDto>>;

public class InitiateVerificationCommandHandler : IRequestHandler<InitiateVerificationCommand, Result<BlockVerificationDto>>
{
    private readonly ICardBlockRepository _repository;

    public InitiateVerificationCommandHandler(ICardBlockRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BlockVerificationDto>> Handle(InitiateVerificationCommand request, CancellationToken cancellationToken)
    {
        var block = await _repository.GetByIdWithVerificationsAsync(request.CardBlockId, cancellationToken);
        if (block is null)
            return Result.Failure<BlockVerificationDto>("Bloke bulunamadı");

        var method = Enumeration.FromId<VerificationMethod>(request.Dto.MethodId);
        if (method is null)
            return Result.Failure<BlockVerificationDto>("Geçersiz doğrulama yöntemi");

        var verificationResult = block.InitiateVerification(method, request.OperatorUsername);
        if (verificationResult.IsFailure)
            return Result.Failure<BlockVerificationDto>(verificationResult.Error);

        var verification = verificationResult.Value!;

        _repository.Update(block);
        await _repository.SaveChangesAsync(cancellationToken);

        return new BlockVerificationDto
        {
            Id = verification.Id,
            CardBlockId = verification.CardBlockId,
            Method = verification.Method.Name,
            MethodDisplayName = verification.Method.DisplayName,
            Result = verification.VerificationResult.Name,
            ResultDisplayName = verification.VerificationResult.DisplayName,
            OtpSentAt = verification.OtpSentAt,
            OtpExpiresAt = verification.OtpExpiresAt,
            OtpAttempts = verification.OtpAttempts,
            IsOtpValid = verification.IsOtpValid(),
            InitiatedAt = verification.InitiatedAt,
            CompletedAt = verification.CompletedAt,
            Notes = verification.Notes,
            AgentUsername = verification.AgentUsername
        };
    }
}