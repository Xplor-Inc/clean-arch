using System.Globalization;

namespace ShareMarket.WebApp.Utilities;

public static class Utility
{
    public static string HexToRGBA(this string hex, decimal alpha = 0.2M)
    {
        if (hex.Contains('#')) { hex = hex.Replace("#", ""); }
        if (hex.Length != 6) return hex;
        var x = Convert.FromHexString(hex);
        return $"rgba({x[0]}, {x[1]}, {x[2]}, {alpha})";
    }
}
