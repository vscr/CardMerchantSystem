using BKM.Domain.Enums;
using BKM.Domain.Events;
using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Entities;

/// <summary>
/// ISO 8583 Switch Mesajı
/// </summary>
public class SwitchMessage : AggregateRoot
{
    // Mesaj Bilgileri
    public MessageType MessageType { get; private set; } = null!;
    public ProcessingCode ProcessingCode { get; private set; } = null!;
    public SwitchMessageStatus Status { get; private set; } = null!;
    public string STAN { get; private set; } = null!; // System Trace Audit Number
    public string RRN { get; private set; } = null!; // Retrieval Reference Number

    // Kart Bilgileri
    public string CardNumberMasked { get; private set; } = null!;
    public string CardNumberEncrypted { get; private set; } = null!;
    public string ExpiryDate { get; private set; } = null!;

    // İşlem Bilgileri
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    public DateTime TransactionDateTime { get; private set; }

    // Terminal/Üye İşyeri Bilgileri
    public string TerminalId { get; private set; } = null!;
    public string MerchantId { get; private set; } = null!;
    public string MCC { get; private set; } = null!; // Merchant Category Code
    public string AcquirerBankCode { get; private set; } = null!;

    // Issuer Bilgileri
    public string IssuerBankCode { get; private set; } = null!;
    public string BIN { get; private set; } = null!;

    // Response Bilgileri
    public ResponseCode? ResponseCode { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Timing
    public DateTime ReceivedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }
    public int ProcessingTimeMs { get; private set; }

    // Routing
    public string? RoutingKey { get; private set; }
    public Guid? OriginalMessageId { get; private set; }

    // EF Core için
    private SwitchMessage() { }

    /// <summary>
    /// Yeni switch mesajı oluşturur
    /// </summary>
    public static Result<SwitchMessage> Create(
        MessageType messageType,
        ProcessingCode processingCode,
        string cardNumberMasked,
        string cardNumberEncrypted,
        string expiryDate,
        decimal amount,
        string currency,
        string terminalId,
        string merchantId,
        string mcc,
        string acquirerBankCode,
        string issuerBankCode)
    {
        if (amount < 0)
            return Result.Failure<SwitchMessage>("Tutar negatif olamaz");

        if (string.IsNullOrWhiteSpace(cardNumberMasked))
            return Result.Failure<SwitchMessage>("Kart numarası boş olamaz");

        var message = new SwitchMessage
        {
            MessageType = messageType,
            ProcessingCode = processingCode,
            Status = SwitchMessageStatus.Received,
            STAN = GenerateSTAN(),
            RRN = GenerateRRN(),
            CardNumberMasked = cardNumberMasked,
            CardNumberEncrypted = cardNumberEncrypted,
            ExpiryDate = expiryDate,
            Amount = amount,
            Currency = currency,
            TransactionDateTime = DateTime.UtcNow,
            TerminalId = terminalId,
            MerchantId = merchantId,
            MCC = mcc,
            AcquirerBankCode = acquirerBankCode,
            IssuerBankCode = issuerBankCode,
            BIN = cardNumberMasked.Replace(" ", "").Substring(0, 6),
            ReceivedAt = DateTime.UtcNow
        };

        message.AddDomainEvent(new SwitchMessageReceivedEvent(
            message.Id,
            messageType.Name,
            message.STAN,
            cardNumberMasked,
            amount));

        return message;
    }

    /// <summary>
    /// Mesajı doğrula
    /// </summary>
    public Result Validate()
    {
        if (Status != SwitchMessageStatus.Received)
            return Result.Failure("Mesaj zaten doğrulanmış");

        // BIN kontrolü
        if (string.IsNullOrEmpty(BIN) || BIN.Length != 6)
            return FailWithResponse(ResponseCode.InvalidCardNumber, "Geçersiz BIN");

        // Tutar kontrolü
        if (ProcessingCode.IsDebit && Amount <= 0)
            return FailWithResponse(ResponseCode.InvalidAmount, "Geçersiz tutar");

        Status = SwitchMessageStatus.Validated;
        return Result.Success();
    }

    /// <summary>
    /// Mesajı yönlendir
    /// </summary>
    public Result Route()
    {
        if (Status != SwitchMessageStatus.Validated)
            return Result.Failure("Mesaj önce doğrulanmalı");

        // Routing key oluştur (BIN bazlı)
        RoutingKey = $"{IssuerBankCode}_{BIN}";

        Status = SwitchMessageStatus.Routed;

        AddDomainEvent(new MessageRoutedEvent(
            Id,
            AcquirerBankCode,
            IssuerBankCode,
            RoutingKey));

        return Result.Success();
    }

    /// <summary>
    /// Authorization işle
    /// </summary>
    public Result ProcessAuthorization(bool isApproved, ResponseCode responseCode, string? authCode = null)
    {
        if (Status != SwitchMessageStatus.Routed)
            return Result.Failure("Mesaj önce yönlendirilmeli");

        Status = SwitchMessageStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        ResponseCode = responseCode;

        if (isApproved)
        {
            AuthorizationCode = authCode ?? GenerateAuthCode();
        }

        AddDomainEvent(new AuthorizationProcessedEvent(
            Id,
            STAN,
            responseCode.Name,
            AuthorizationCode ?? "",
            isApproved));

        return Result.Success();
    }

    /// <summary>
    /// Response gönder
    /// </summary>
    public Result SendResponse()
    {
        if (Status != SwitchMessageStatus.Processed && Status != SwitchMessageStatus.Failed)
            return Result.Failure("Mesaj önce işlenmeli");

        Status = SwitchMessageStatus.Responded;
        RespondedAt = DateTime.UtcNow;
        ProcessingTimeMs = (int)(RespondedAt.Value - ReceivedAt).TotalMilliseconds;

        return Result.Success();
    }

    /// <summary>
    /// Timeout olarak işaretle
    /// </summary>
    public Result MarkAsTimeout()
    {
        if (Status.IsFinal)
            return Result.Failure("Mesaj zaten sonlanmış");

        Status = SwitchMessageStatus.Timeout;
        ResponseCode = Enums.ResponseCode.Timeout;
        ErrorMessage = "İşlem zaman aşımına uğradı";
        ProcessedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private Result FailWithResponse(ResponseCode code, string message)
    {
        Status = SwitchMessageStatus.Failed;
        ResponseCode = code;
        ErrorMessage = message;
        ProcessedAt = DateTime.UtcNow;
        return Result.Failure(message);
    }

    private static string GenerateSTAN()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    private static string GenerateRRN()
    {
        return $"{DateTime.UtcNow:yyMMdd}{new Random().Next(100000, 999999)}";
    }

    private static string GenerateAuthCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}