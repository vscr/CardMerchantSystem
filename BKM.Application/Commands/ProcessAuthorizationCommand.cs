using BKM.Application.DTOs;
using BKM.Domain.Entities;
using BKM.Domain.Enums;
using BKM.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Commands;

public record ProcessAuthorizationCommand(AuthorizationRequestDto Request) : IRequest<Result<AuthorizationResponseDto>>;
public class ProcessAuthorizationCommandHandler
    : IRequestHandler<ProcessAuthorizationCommand, Result<AuthorizationResponseDto>>
{
    private readonly ISwitchMessageRepository _messageRepository;
    private readonly IBINTableRepository _binRepository;

    public ProcessAuthorizationCommandHandler(
        ISwitchMessageRepository messageRepository,
        IBINTableRepository binRepository)
    {
        _messageRepository = messageRepository;
        _binRepository = binRepository;
    }

    public async Task<Result<AuthorizationResponseDto>> Handle(
        ProcessAuthorizationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Request;
        var startTime = DateTime.UtcNow;

        // 1. BIN kontrolü - Issuer bankayı bul
        var cardBin = dto.CardNumber.Replace(" ", "").Substring(0, 6);
        var binInfo = await _binRepository.GetByBINAsync(cardBin, cancellationToken);

        var issuerBankCode = binInfo?.BankCode ?? "UNKNOWN";

        // 2. Kart numarasını maskele ve şifrele
        var cardNumberClean = dto.CardNumber.Replace(" ", "");
        var maskedCard = $"{cardNumberClean.Substring(0, 4)} **** **** {cardNumberClean.Substring(12, 4)}";
        var encryptedCard = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cardNumberClean)); // Basit şifreleme

        // 3. Switch mesajı oluştur
        var messageResult = SwitchMessage.Create(
            MessageType.Authorization,
            ProcessingCode.Purchase,
            maskedCard,
            encryptedCard,
            dto.ExpiryDate,
            dto.Amount,
            dto.Currency,
            dto.TerminalId,
            dto.MerchantId,
            dto.MCC,
            dto.AcquirerBankCode,
            issuerBankCode);

        if (messageResult.IsFailure)
            return Result.Failure<AuthorizationResponseDto>(messageResult.Error!);

        var message = messageResult.Value!;

        // 4. Validate
        var validateResult = message.Validate();
        if (validateResult.IsFailure)
        {
            await SaveAndRespond(message, cancellationToken);
            return CreateErrorResponse(message, startTime);
        }

        // 5. Route
        var routeResult = message.Route();
        if (routeResult.IsFailure)
        {
            message.ProcessAuthorization(false, ResponseCode.SystemMalfunction);
            await SaveAndRespond(message, cancellationToken);
            return CreateErrorResponse(message, startTime);
        }

        // 6. Simüle authorization (gerçekte issuer bankaya gidecek)
        var authResult = SimulateIssuerAuthorization(dto, binInfo);

        message.ProcessAuthorization(authResult.IsApproved, authResult.ResponseCode, authResult.AuthCode);

        // 7. Send response
        message.SendResponse();

        await _messageRepository.AddAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        var processingTime = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;

        return new AuthorizationResponseDto
        {
            IsApproved = authResult.IsApproved,
            ResponseCode = message.ResponseCode!.Name,
            ResponseMessage = message.ResponseCode!.DisplayName,
            AuthorizationCode = message.AuthorizationCode,
            STAN = message.STAN,
            RRN = message.RRN,
            ProcessingTimeMs = processingTime,
            MessageId = message.Id
        };
    }

    private async Task SaveAndRespond(SwitchMessage message, CancellationToken cancellationToken)
    {
        message.SendResponse();
        await _messageRepository.AddAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);
    }

    private Result<AuthorizationResponseDto> CreateErrorResponse(SwitchMessage message, DateTime startTime)
    {
        var processingTime = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
        return new AuthorizationResponseDto
        {
            IsApproved = false,
            ResponseCode = message.ResponseCode?.Name ?? "96",
            ResponseMessage = message.ResponseCode?.DisplayName ?? "Sistem hatası",
            AuthorizationCode = null,
            STAN = message.STAN,
            RRN = message.RRN,
            ProcessingTimeMs = processingTime,
            MessageId = message.Id
        };
    }

    private (bool IsApproved, ResponseCode ResponseCode, string? AuthCode) SimulateIssuerAuthorization(
        AuthorizationRequestDto dto, BINTable? binInfo)
    {
        // BIN bulunamadıysa
        if (binInfo == null || !binInfo.IsActive)
            return (false, ResponseCode.NoSuchIssuer, null);

        // Expiry date kontrolü (MMYY format)
        if (!ValidateExpiryDate(dto.ExpiryDate))
            return (false, ResponseCode.ExpiredCard, null);

        // CVV kontrolü (basit simülasyon - 000 = hatalı)
        if (dto.CVV == "000")
            return (false, ResponseCode.SecurityViolation, null);

        // Tutar kontrolü
        if (dto.Amount > 50000)
            return (false, ResponseCode.ExceedsLimit, null);

        // Random red (test için %5 red)
        if (new Random().Next(100) < 5)
            return (false, ResponseCode.DoNotHonor, null);

        // Onay
        var authCode = GenerateAuthCode();
        return (true, ResponseCode.Approved, authCode);
    }

    private bool ValidateExpiryDate(string expiryDate)
    {
        if (expiryDate.Length != 4)
            return false;

        if (!int.TryParse(expiryDate.Substring(0, 2), out var month) ||
            !int.TryParse(expiryDate.Substring(2, 2), out var year))
            return false;

        var expiry = new DateTime(2000 + year, month, 1).AddMonths(1).AddDays(-1);
        return expiry >= DateTime.UtcNow;
    }

    private string GenerateAuthCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}