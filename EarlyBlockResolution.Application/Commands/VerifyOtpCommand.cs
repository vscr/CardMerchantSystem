using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Commands;

public record VerifyOtpCommand(Guid CardBlockId, VerifyOtpDto Dto) : IRequest<Result<BlockVerificationDto>>;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<BlockVerificationDto>>
{
    private readonly ICardBlockRepository _repository;

    public VerifyOtpCommandHandler(ICardBlockRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BlockVerificationDto>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var block = await _repository.GetByIdWithVerificationsAsync(request.CardBlockId, cancellationToken);
        if (block is null)
            return Result.Failure<BlockVerificationDto>("Bloke bulunamadı");

        var verification = block.Verifications.FirstOrDefault(v => v.Id == request.Dto.VerificationId);
        if (verification is null)
            return Result.Failure<BlockVerificationDto>("Doğrulama bulunamadı");

        var verifyResult = verification.VerifyOtp(request.Dto.OtpCode);
        if (verifyResult.IsFailure)
        {
            _repository.Update(block);
            await _repository.SaveChangesAsync(cancellationToken);
            return Result.Failure<BlockVerificationDto>(verifyResult.Error);
        }

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