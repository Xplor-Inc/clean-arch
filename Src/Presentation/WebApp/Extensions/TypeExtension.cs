namespace ShareMarket.WebApp.Extensions
{
    public static class TypeExtension
    {
        public static string GetDateInStringFormat(this DateTime dt)
        {
            return dt.ToString("dd MMM yy");
        }
        public static string GetAmountInStringFormat(this decimal para)
        {
            if (para > 0 && para <= 100)
                return para.ToString("#,##0.##");

            if (para > 100 && para <= 1000)
                return para.ToString("#,##0.#");

            return para.ToString("#,##0");
        }

        public static string GetAmountInStringFormat(this int para)
        {
            if (para > 0 && para <= 100)
                return para.ToString("#,##0.##");

            if (para > 100 && para <= 1000)
                return para.ToString("#,##0.#");

            return para.ToString("#,##0");
        }
        public static string RoundGetString(this decimal para, int places)
        {
            if (places == 1)
                return para.ToString("#,##0.#");
            if (places == 2)
                return para.ToString("#,##0.##");
            if (places == 3)
                return para.ToString("#,##0.###");
            return para.ToString("#,##0");
        }

        public static string GetDayMonthYear(this decimal para)
        {
            int days = Convert.ToInt32(para);
            if (para <= 30)
            {
                return $"{days}D";
            }
            if (para <= 365)
            {
                int month = days / 30;
                int day = (days % 30);

                return $"{month}M,{day}D";
            }
            else
            {
                int year = Convert.ToInt32(days / 365);
                int month = Convert.ToInt32(days % 365);
                int day = Convert.ToInt32(days % 30);
                return $"{year}Y,{month}M,{day}D";
            }

        }
        public static string GetDayMonthYear(this int para)
        {
            int days = para;//Convert.ToInt32(para);
            if (para <= 30)
            {
                return $"{days}D";
            }
            if (para <= 365)
            {
                int month = days / 30;
                int day = (days % 30);

                return $"{month}M,{day}D";
            }
            else
            {
                int year = Convert.ToInt32(days / 365);
                int month = Convert.ToInt32(days % 365);
                int day = Convert.ToInt32(days % 30);
                return $"{year}Y,{month}M,{day}D";
            }

        }
        public static string FormatLargeNumber(this decimal num)
        {
            
            if (num >= 100000)
                return FormatLargeNumber(num / 1000) + "K";

            if (num >= 10000)
                return ((long)num / 1000D).ToString("0.#") + "K";

            return num.ToString("#,0");
        }
    }
}
