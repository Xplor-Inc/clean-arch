using System.Globalization;

namespace ShareMarket.Core.Extensions;

public static class DateTimeExtensions
{
    static readonly CultureInfo format = CultureInfo.InvariantCulture;

    public static DateTimeOffset? ToDateTimeOffset(this DateOnly? _)
    {
        if (!_.HasValue) return null;
        return new DateTimeOffset(_.Value.Year, _.Value.Month, _.Value.Day, 0, 0, 0, new TimeSpan(5, 30, 0));
    }
    public static DateOnly BudgetMonth(this DateTimeOffset dateOnly)
    {
        dateOnly = dateOnly.ToIst();
        return new DateOnly(dateOnly.Year, dateOnly.Month, 1);
    }
    public static DateOnly ToDateOnly(this DateTimeOffset _)
    {
        _ = _.ToIst();
        return new DateOnly(_.Year, _.Month, _.Day);
    }
    public static DateOnly BudgetMonth(this DateOnly dateOnly)
    {
        return new DateOnly(dateOnly.Year, dateOnly.Month, 1);
    }
    public static DateTimeOffset ToIst(this DateTimeOffset _)
    {
        return new DateTimeOffset(_.ToUniversalTime().AddHours(5).AddMinutes(30).DateTime, new TimeSpan(5, 30, 0));
    }
    public static DateTimeOffset ToDateTimeOffset(this DateOnly _)
    {
        return new DateTimeOffset(_.Year,_.Month, _.Day,0,0,0, new TimeSpan(5, 30, 0));
    }
    public static DateTime ToIst(this DateTime _)
    {
        return _.ToUniversalTime().AddHours(5).AddMinutes(30);
    }
    public static string ToFullDateString(this DateTimeOffset _)
    {
        return _.ToIst().ToString("dddd, dd MMM yyyy, hh:mm:ss tt", format);
    }
    public static string ToTimeString(this DateTimeOffset _)
    {
        return _.ToIst().ToString("hh:mm:ss tt", format);
    }
    public static string ToDateTimeString(this DateTimeOffset _)
    {
        return _.ToIst().ToString("dd MMM yyyy, hh:mm:ss tt", format);
    }
    public static string ToDateString(this DateTimeOffset _)
    {
        return _.ToIst().ToString("dd MMM yyyy", format);
    }
    public static string ToString_DD_MMM(this DateTimeOffset _)
    {
        return _.ToIst().ToString("dd MMM", format);
    }
    public static string ToString_MMM_YYYY(this DateTime _)
    {
        return _.ToString("MMM yy", format);
    }
}
