namespace ShareMarket.Core.Extensions;

public static class StringExtension
{
    public static bool IsInValid(this string? val)
    {
        return string.IsNullOrWhiteSpace(val);
    }
}