using ShareMarket.Core.Extensions;

namespace ShareMarket.Core.Utilities;

public static class CommonUtils
{

    public static List<DateOnly> GetMonths(DateOnly? from, DateOnly? to)
    {
        List<DateOnly> months = [];
        if (from is null && to is null) return months;
        if (!to.HasValue) to = DateOnly.FromDateTime(DateTimeOffset.Now.ToIst().Date);
        if (!from.HasValue) from = DateOnly.FromDateTime(DateTimeOffset.Now.ToIst().Date);

        while (from <= to)
        {
            months.Add(new DateOnly(from.Value.Year, from.Value.Month, 1));
            from = from.Value.AddMonths(1);
        }
        return months;
    }

}
