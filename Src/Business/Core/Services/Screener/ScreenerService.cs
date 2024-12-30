using HtmlAgilityPack;
using ShareMarket.Core.Entities.Equities;
using System.Net;

namespace ShareMarket.Core.Services.Screener;

public class ScreenerService(IRepositoryConductor<EquityHistorySyncLog> ErrorLogRepo) : IScreenerService
{
    private static readonly HttpClient Client = new() { BaseAddress = new Uri("https://www.screener.in/") };
    public async Task<decimal> SyncShareHoldingByScreener(EquityStock stock)
    {
        var url = $"company/{stock.Code}/consolidated/";
        decimal r = 0;
        try
        {
            var response = await Client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var doc = new HtmlDocument();

                var pageContents = response.Content.ReadAsStringAsync().Result;
                doc.LoadHtml(pageContents);
                var table = doc.GetElementbyId("quarterly-shp");
                if (table == null)
                {
                    await CreateSyncErrorLog(stock.Code, url, "quarterly-shp not found in response string");
                    return r;
                }
                doc.LoadHtml(table.InnerHtml);
                var delTable = doc.DocumentNode.SelectNodes("//tbody")[0];
                var delTable1 = table.SelectNodes("//table");
                var rows = delTable.ChildNodes.ToList().Where(x => x.Name == "tr");
                foreach (var row in rows)
                {
                    var colums = row.ChildNodes.ToList().Where(x => x.Name == "td");
                    var prm = colums.First().InnerText.Replace("\n","").Replace("  ", "").Replace("+", "").Replace("&nbsp;", "");
                    var vl = colums.Last().InnerText.Replace("%", "");
                    if (string.Equals(prm, "Promoters", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrWhiteSpace(vl))
                    {
                        return decimal.Parse(vl);
                    }
                    else
                    {
                        await CreateSyncErrorLog(stock.Code, url, "quarterly-shp Node found but Promoters not found in response string");
                        return r;
                    }
                }
            }
            else
            {
                var er = await response.Content.ReadAsStringAsync();
                await CreateSyncErrorLog(stock.Code, url, $"API Error: {er}");
                return r;
            }
        }
        catch (Exception ex)
        {
            await CreateSyncErrorLog(stock.Code, url, ex.ToString());
            return r;
        }
        return r;
    }
    public async Task CreateSyncErrorLog(string code, string provider, string error)
    {
        EquityHistorySyncLog errorLog = new()
        {
            Name = code,
            Code = code,
            Provider = provider,
            ErrorMessage = error,
        };

        var createResult = await ErrorLogRepo.CreateAsync(errorLog, SystemConstant.SystemUserId);
        if (createResult != null)
        {
        }
    }

}
