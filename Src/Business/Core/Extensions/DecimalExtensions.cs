using System.Globalization;

namespace ShareMarket.Core.Extensions;

public static class DecimalExtensions
{

    readonly static CultureInfo Culture = new ("en-IN", true);
    readonly static string format = "₹ ###,##0.##";
    public static string ToPString(this decimal d, int decimalPoint = 2)
    {
        return $"{d.ToFixed(decimalPoint).ToString("###,##0.#0", Culture)}%";
    }
    public static string ToPString(this decimal? d)
    {
        return $"{d?.ToString("###,##0.##", Culture)}%";
    }
    public static string ToCString(this decimal? d)
    {
        return d.HasValue ? d.Value.ToString(format, Culture) : string.Empty;
    }
    public static string ToCString(this decimal d)
    {
        return d.ToString(format, Culture);
    }
    public static string ToCString(this decimal d, int decimalPoint)
    {
        return d.ToFixed(decimalPoint).ToString(format, Culture);
    }

    public static string ToCString(this double d)
    {
        return d.ToString(format, Culture);
    }
    public static decimal ToFixed(this decimal d, int decimalPoint = 2)
    {
        return decimal.Round(d, decimalPoint, MidpointRounding.AwayFromZero);
    }
    public static string ToFixedString(this decimal d, int decimalPoint = 2)
    {
        return decimal.Round(d, decimalPoint, MidpointRounding.AwayFromZero).ToString(format, Culture); ;
    }
    public static string ToKMB(this decimal num)
    {
        var info = new CultureInfo("en-IN");
        if (num > 9999999)
        {
            return (num / 1000000).ToFixed(0).ToString(info) + "Cr";
        }
        else if (num > 99999)
        {
            return (num / 100000).ToFixed(0).ToString(info)+"L";
        }
        else if (num > 999)
        {
            return (num/1000).ToFixed(0).ToString(info)+"K";
        }
        else
        {
            return num.ToString(info);
        }

        //if (num > 999999999 || num < -999999999)
        //{
        //    return num.ToString("0,,,.###B", info);
        //}
        //else if (num > 99999 || num < -99999)
        //{
        //    return num.ToString("0,,.#L", info);
        //}
        //else if (num > 999 || num < -999)
        //{
        //    return num.ToString("0,.#K", info);
        //}
        //else
        //{
        //    return num.ToString(info);
        //}
    }
}