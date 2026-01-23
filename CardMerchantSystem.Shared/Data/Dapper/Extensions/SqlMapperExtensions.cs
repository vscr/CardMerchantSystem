// CardMerchantSystem.Shared/Data/Dapper/Extensions/SqlMapperExtensions.cs

using System.Data;
using Dapper;

namespace CardMerchantSystem.Shared.Data.Dapper.Extensions;

/// <summary>
/// Dapper type handler'lar ve custom mapping'ler
/// </summary>
public static class SqlMapperExtensions
{
    /// <summary>
    /// Guid type handler - SQL Server için
    /// </summary>
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            return Guid.Parse(value.ToString()!);
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
        }
    }

    /// <summary>
    /// Nullable Guid type handler
    /// </summary>
    public class NullableGuidTypeHandler : SqlMapper.TypeHandler<Guid?>
    {
        public override Guid? Parse(object value)
        {
            if (value == null || value is DBNull)
                return null;

            return Guid.Parse(value.ToString()!);
        }

        public override void SetValue(IDbDataParameter parameter, Guid? value)
        {
            parameter.Value = value?.ToString() ?? (object)DBNull.Value;
        }
    }

    /// <summary>
    /// DateTime type handler - UTC conversion
    /// </summary>
    public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override DateTime Parse(object value)
        {
            var dateTime = (DateTime)value;
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value.ToUniversalTime();
        }
    }

    /// <summary>
    /// Type handler'ları kaydeder
    /// </summary>
    public static void RegisterTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
        SqlMapper.AddTypeHandler(new NullableGuidTypeHandler());
        SqlMapper.AddTypeHandler(new DateTimeHandler());
    }

    /// <summary>
    /// Column mapping için case-insensitive ayarlar
    /// </summary>
    public static void ConfigureCaseInsensitiveMapping()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}