using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Extensions;
using System.Net;

namespace ShareMarket.Core.Services.EquityPandit;

public class EquityPanditService(
    IRepositoryConductor<EquityPriceHistory>    PriceHistoryRepo,
    IRepositoryConductor<EquityHistorySyncLog>  ErrorLogRepo) : IEquityPanditService
{
    readonly HttpClient Client = new() { BaseAddress = new Uri("https://www.equitypandit.com") };

    public async Task<Result<List<EquityPriceHistory>>> SyncPriceEquityPandit(EquityStock stock)
    {
        var url = $"/historical-data/{stock.Code}";
        Result<List<EquityPriceHistory>> responseObj = new([]);
        int attempt = 1;
        while (attempt <= 2)
        {
        try
        {
            DateOnly startDate;
            var history_Old = await PriceHistoryRepo.FindAll(x => x.Code == stock.Code)
                                    .ResultObject.FirstOrDefaultAsync();
                startDate = history_Old?.Date ?? new DateOnly(2022, 1, 1);

            var response = await Client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var doc = new HtmlDocument();

                var pageContents = response.Content.ReadAsStringAsync().Result;
                doc.LoadHtml(pageContents);
                var table = doc.GetElementbyId("testTable2");
                if (table == null)
                {
                    await CreateSyncErrorLog(stock.Code, url, "testTable2 not found in response string");
                        url = $"/historical-data/{stock.BSECode}";
                        attempt++;
                        continue;
                }

                var delTable = table.SelectNodes("//tbody")[0];
                var rows = delTable.ChildNodes.ToList().Where(x => x.Name == "tr");
                foreach (var row in rows)
                {
                    var colums = row.ChildNodes;
                    var date = DateOnly.Parse(colums[0].InnerText);
                    if (date < startDate)
                        continue;

                    var close   = decimal.Parse(colums[1].InnerText);
                    var open    = decimal.Parse(colums[2].InnerText);
                    var high    = decimal.Parse(colums[3].InnerText);
                    var low     = decimal.Parse(colums[4].InnerText);
                    var pChange = decimal.Parse(colums[6].InnerText.Replace("%",""));

                        var dayChange = (close / (1 + pChange / 100)) - close;

                        responseObj.ResultObject.Add(new EquityPriceHistory
                    {
                        Date            = date,
                        Close           = close,
                        Open            = open,
                        High            = high,
                        Low             = low,
                        Code            = stock.Code,
                        Name            = stock.Name,
                        EquityId        = stock.Id,
                        DayChange       = dayChange,
                        DayChangePer    = pChange
                    });
                }
            }
            else
            {
                var er = await response.Content.ReadAsStringAsync();
                await CreateSyncErrorLog(stock.Code, url, $"API Error: {er}");
                    url = $"/historical-data/{stock.BSECode}";
                    attempt++;
                    continue;
                    //return responseObj;
            }
                break; // break loop
        }
        catch (Exception ex)
        {
                url = $"/historical-data/{stock.BSECode}";
                attempt++;
            await CreateSyncErrorLog(stock.Code, url, ex.ToString());
        }

    }

        return responseObj;
    }

    public async Task<Result<EquityStock>> SyncFundamentalEquityPandit(EquityStock stock)
    {
        Result<EquityStock> result = new(stock);

        var url = $"/share-price/{stock.Code}";
        int attempt = 1;
        while (attempt <= 2)
        {
        try
        {
            var response = await Client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var doc = new HtmlDocument();

                var pageContents = response.Content.ReadAsStringAsync().Result;
                doc.LoadHtml(pageContents);
                var table = doc.GetElementbyId("ratio_section");
                if (table is null)
                {
                    result.AddError($"Page content not found at Node ratio_section");
                    return result;
                }
                IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("tx-uppercase")).ToList();
                foreach (var item in nodes)
                {
                    if (item.InnerText == "EPS")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.EPS = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "P/E")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.PE = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "P/B")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.PD = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "Dividend Yield")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.Dividend = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "Market Cap")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.MarketCap = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "Face Value")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.FaceValue = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "Book Value")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.BookValue = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "Debt/Equity")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.DebtEquity = decimal.Parse(value);
                        continue;
                    }
                    if (item.InnerText == "ROE")
                    {
                        string value = item.NextSibling.NextSibling.FirstChild.InnerText;
                        stock.ROE = decimal.Parse(value);
                        continue;
                    }
                }

                //Read Technical Analysis
                var x = doc.GetElementbyId("technical_analysis")?.ChildNodes.Where(x=>x.Name == "div").ToList();
                if(x?.Count == 4)
                {
                    var ds = x[1].ChildNodes.Where(x => x.Name == "div").ToList();
                    if(ds.Count == 3)
                    {
                        var i0 = ds[0].ChildNodes.Where(x => x.Name == "h5").ToList();
                        if(i0.Count == 2)
                        {
                            stock.ShortTerm = i0[1].InnerText;
                        }
                        var i1 = ds[1].ChildNodes.Where(x => x.Name == "h5").ToList();
                        if (i1.Count == 2)
                        {
                            stock.MediumTerm = i1[1].InnerText;
                        }
                        var i2 = ds[2].ChildNodes.Where(x => x.Name == "h5").ToList();
                        if (i2.Count == 2)
                        {
                            stock.LongTerm = i2[1].InnerText;
                        }
                    }
                }
                //Shareholding Pattern
                var u = doc.GetElementbyId("share_holding")?.ChildNodes[1].ChildNodes.Where(x => x.Name == "div").ToList();
                if(u?.Count == 3)
                {
                    var t = u[2].ChildNodes.ToList();
                }

                var sdf = u;
                
            }
            else
            {
                result.AddError($"API Error Url :{url}, {await response.Content.ReadAsStringAsync()}");
                    url = $"/historical-data/{stock.BSECode}";
                    attempt++;
                    continue;
            }
                break;
        }
        catch (Exception ex)
        {
                url = $"/historical-data/{stock.BSECode}";
                attempt++;
            result.AddError($"Exception at SyncFundamentalEquityPandit: {ex.Message}");
                continue;
            }
        }
        return result;
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