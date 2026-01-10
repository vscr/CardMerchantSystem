using HSM.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Entities;

/// <summary>
/// HSM Komut Log Kaydı
/// </summary>
public class HSMCommandLog : Entity
{
    public Guid HSMDeviceId { get; private set; }
    public HSMCommandType CommandType { get; private set; } = null!;
    public string RequestData { get; private set; } = null!;
    public string? ResponseData { get; private set; }
    public string? ResponseCode { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int ExecutionTimeMs { get; private set; }
    public DateTime ExecutedAt { get; private set; }
    public string? ReferenceId { get; private set; }
    public string? CardNumberMasked { get; private set; }

    // EF Core için
    private HSMCommandLog() { }

    public static HSMCommandLog Create(
        Guid hsmDeviceId,
        HSMCommandType commandType,
        string requestData,
        string? referenceId = null,
        string? cardNumberMasked = null)
    {
        return new HSMCommandLog
        {
            HSMDeviceId = hsmDeviceId,
            CommandType = commandType,
            RequestData = MaskSensitiveData(requestData),
            ExecutedAt = DateTime.UtcNow,
            ReferenceId = referenceId,
            CardNumberMasked = cardNumberMasked
        };
    }

    public void SetResponse(string responseData, string responseCode, bool isSuccess, int executionTimeMs, string? errorMessage = null)
    {
        ResponseData = MaskSensitiveData(responseData);
        ResponseCode = responseCode;
        IsSuccess = isSuccess;
        ExecutionTimeMs = executionTimeMs;
        ErrorMessage = errorMessage;
    }

    private static string MaskSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return data;

        // PIN ve key değerlerini maskele
        // Gerçek implementasyonda daha detaylı maskeleme yapılmalı
        if (data.Length > 32)
        {
            return data.Substring(0, 16) + "****" + data.Substring(data.Length - 8);
        }
        return data;
    }
}