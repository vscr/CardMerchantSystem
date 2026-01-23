// CardMerchantSystem.Shared/Data/Dapper/Extensions/DapperExtensions.cs

using Dapper;

namespace CardMerchantSystem.Shared.Data.Dapper.Extensions;

/// <summary>
/// Dapper için extension metodlar
/// </summary>
public static class DapperExtensions
{
    /// <summary>
    /// Dynamic parameters'a null-safe değer ekler
    /// </summary>
    public static DynamicParameters AddParameterIfNotNull(
        this DynamicParameters parameters,
        string name,
        object? value)
    {
        if (value != null)
        {
            parameters.Add(name, value);
        }
        return parameters;
    }

    /// <summary>
    /// Optional filter parametreleri ekler
    /// </summary>
    public static DynamicParameters AddOptionalParameter<T>(
        this DynamicParameters parameters,
        string name,
        T? value) where T : struct
    {
        if (value.HasValue)
        {
            parameters.Add(name, value.Value);
        }
        return parameters;
    }

    /// <summary>
    /// Liste parametresi ekler (IN clause için)
    /// </summary>
    public static DynamicParameters AddListParameter<T>(
        this DynamicParameters parameters,
        string name,
        IEnumerable<T>? values)
    {
        if (values != null && values.Any())
        {
            parameters.Add(name, values);
        }
        return parameters;
    }

    /// <summary>
    /// Date range parametreleri ekler
    /// </summary>
    public static DynamicParameters AddDateRange(
        this DynamicParameters parameters,
        DateTime? startDate,
        DateTime? endDate,
        string startParamName = "@StartDate",
        string endParamName = "@EndDate")
    {
        if (startDate.HasValue)
        {
            parameters.Add(startParamName, startDate.Value);
        }
        if (endDate.HasValue)
        {
            parameters.Add(endParamName, endDate.Value);
        }
        return parameters;
    }
}