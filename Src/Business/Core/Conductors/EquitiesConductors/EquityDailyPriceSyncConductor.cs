using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShareMarket.Core.Entities;
using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Entities.Schemes;
using ShareMarket.Core.Enumerations;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Interfaces.Hubs;
using ShareMarket.Core.Interfaces.SqlServer;
using ShareMarket.Core.Models.Groww;
using ShareMarket.Core.Models.Services.Groww;

namespace ShareMarket.Core.Conductors.EquitiesConductors;

public class EquityDailyPriceSyncConductor(
    INotificationHub                            NotificationHub,
    IRepositoryConductor<EquityStock>           EquityRepo, 
    IRepositoryConductor<EquityPriceHistory>    HistoryRepo,
    IRepositoryConductor<SchemeEquityHolding>   SchemeHoldingRepo,
    IRepositoryConductor<Scheme>                SchemeRepo,
    IEquityTechnicalCalulationConductor         TechnicalCalulationConductor,
    IGrowwService                               GrowwService,
    IEquityPanditService                        EquityPanditService,
    IRepository<WatchlistStock>                 watchlistRepoStock,
    IScreenerService                            ScreenerService,
    IRepositoryConductor<EquityStockCalculation> _EquityStockCalculationRepo
    ) : IEquityDailyPriceSyncConductor
{
    public async Task UpdateRank()
    {
        var holding = await SchemeHoldingRepo.FindAll(e => e.DeletedOn == null).ResultObject.Select(s => new {s.SectorName, s.SchemeId, s.Code, s.CompanyName }).ToListAsync();

        var stocks = await EquityRepo.FindAll(s => s.DeletedOn == null).ResultObject.ToListAsync();
        int count = 0;
        foreach (var stock in stocks)
        {
            count++;
            var z = holding.Count(c => string.Equals(c.Code, stock.Code));
            stock.GrowwRank = holding.Count(c => string.Equals(c.Code, stock.Code));
            stock.Sector = holding.FirstOrDefault(x => x.SectorName != null && x.Code == stock.Code)?.SectorName;
            stock.IsActive = stock.GrowwRank > 0;
        }
        var updateResult = await EquityRepo.UpdateAsync(stocks, SystemConstant.SystemUserId);
    }

    public async Task SynchStockListByWatchList()
    {
        
        var  result =await watchlistRepoStock.FindAll(includeProperties: "EquityStock,WatchList").ResultObject.ToListAsync();
        var lstWatchlistStock = result.DistinctBy(x => x.EquityStockId).ToList();
        await NotificationHub.WatchlistSync($"LTP Data Processing..", "started");
        await SyncStockPrice_Groww(lstWatchlistStock.Select(x=>x.EquityStockId).ToList(),CancellationToken.None);

        await ResetEquietyMaster(lstWatchlistStock.Select(x => x.EquityStockId).ToList());
        await ResetEqityStockCalculations(lstWatchlistStock.Select(x => (long?)x.EquityStockId).ToList());

        await SyncStockPrice_Groww(lstWatchlistStock.Select(x => x.EquityStockId).ToList(), CancellationToken.None);

        int count = 1;
        foreach (var EquityStock in lstWatchlistStock.Select(x=>x.EquityStock))
        {
            if (EquityStock is null) continue;
            EquityStock.WatchlistStockList = null;
            EquityStock.EquityStockCalculation = null;
            string strMessage = $"{EquityStock.Code} - {count} of {lstWatchlistStock.Count}";
           
            //await SyncHistoryByPandit(EquityStock, $"History {strMessage}");
            //await _EquityTechnicalCalulationConductor.SyncEquietyStockCalculation(EquityStock, $"Calculation {strMessage}");
            //await FundamentalSyncByPandit(EquityStock, $"Fundaental {strMessage}");
            BackgroundJob.Enqueue<EquityDailyPriceSyncConductor>(x => SyncHistoryByPandit(EquityStock, $"History {strMessage}"));
            BackgroundJob.Enqueue<EquityTechnicalCalulationConductor>(x => x.SyncEquietyStockCalculation(EquityStock, $"Calculation {strMessage}"));
            BackgroundJob.Enqueue<EquityDailyPriceSyncConductor>(x => x.FundamentalSyncByPandit(EquityStock, $"Fundaental {strMessage}"));
            count++;
        }

     }

    private async Task ResetEquietyMaster(List<long> Ids)
    {
        var EqityStocks = await EquityRepo.FindAll(x => Ids.Contains(x.Id),asNoTracking:false).ResultObject.ToListAsync();

        foreach (var EqityStock in EqityStocks)
        {
            EqityStock.EPS = 0;
            EqityStock.PE = 0;
            EqityStock.PD= 0;
            EqityStock.Dividend = 0;
            EqityStock.MarketCap = 0;
            EqityStock.FaceValue= 0;
            EqityStock.BookValue = 0;
            EqityStock.DebtEquity = 0;
            EqityStock.ROE = 0;
           
            EqityStock.ShortTerm = null;
            EqityStock.LongTerm = null;
            EqityStock.MediumTerm = null;

            EqityStock.DayChange = 0;
            EqityStock.DayHigh = 0;
            EqityStock.DayLow = 0;
            EqityStock.LTP = 0;
            EqityStock.LastSyncOn = null;
            EqityStock.MarketCap = 0;
            EqityStock.DayChangePer = 0;

            
        }
        var resp = await EquityRepo.UpdateAsync(EqityStocks, SystemConstant.SystemUserId);
        if (resp.HasErrors)
        {
            throw new Exception(string.Join(",", resp.Errors));
        }
    }
   
    private async Task ResetEqityStockCalculations(List<long?> Ids)
    {

        var EqityStockCalculations = await _EquityStockCalculationRepo.FindAll(x => Ids.Contains(x.EquityStockId),asNoTracking:false).ResultObject.ToListAsync();

        foreach (var EqityStockCalculation in EqityStockCalculations)
        {
            EqityStockCalculation.RSI14EMA = 0;
            EqityStockCalculation.RSI14EMADiff = 0;
            EqityStockCalculation.RSI = 0;
            EqityStockCalculation.DMA5 = 0;
            EqityStockCalculation.DMA10 = 0;
            EqityStockCalculation.DMA20 = 0;
            EqityStockCalculation.DMA50 = 0;
            EqityStockCalculation.DMA100 = 0;
            EqityStockCalculation.DMA200 = 0;
            EqityStockCalculation.YearHigh = 0;
            EqityStockCalculation.YearHighOn = null;
            EqityStockCalculation.YearLow = 0;
            EqityStockCalculation.YearLowOn = null;
            EqityStockCalculation.IsRaising = false;

            #region Stock Growth
            EqityStockCalculation.PerChange_1W = 0;
            EqityStockCalculation.PerChange_15D = 0;
            EqityStockCalculation.PerChange_1M = 0;
            EqityStockCalculation.PerChange_3M = 0;
            EqityStockCalculation.PerChange_6M = 0;
            EqityStockCalculation.PerChange_1Y = 0;
            #endregion           
        }
            
        var resp = await _EquityStockCalculationRepo.UpdateAsync(EqityStockCalculations, SystemConstant.SystemUserId);
        if (resp.HasErrors)
        {
            throw new Exception(string.Join(",", resp.Errors));
     }
    }
    
    public async Task<bool> SyncStockPrice_Groww(List<long> lstStockId,CancellationToken cancellationToken)
    {
        var r = new Result<bool>();
        var scripResult = await GrowwService.GetScrips(cancellationToken);

        var StockToSync = EquityRepo.FindAll(x => lstStockId.Contains(x.Id),asNoTracking:false).ResultObject.ToList();
        foreach (var equityStock in StockToSync)
        {
            
           var GrowwStockDetail = scripResult.ResultObject.Where(x => x.NseScriptCode == equityStock.Code || x.BseScriptCode == equityStock.Code).FirstOrDefault();
            if (GrowwStockDetail == null)
                continue;

            equityStock.DayChange = GrowwStockDetail.LivePriceDto.DayChange;
            equityStock.DayHigh = GrowwStockDetail.LivePriceDto.High;
            equityStock.DayLow = GrowwStockDetail.LivePriceDto.Low;
            equityStock.LTP = GrowwStockDetail.LivePriceDto.Ltp;
            equityStock.DayChangePer = GrowwStockDetail.LivePriceDto.DayChangePerc;
            equityStock.LastSyncOn=DateTime.Now;
            var resp = await EquityRepo.UpdateAsync(equityStock, 1, cancellationToken);
            if (resp.HasErrors)
            {
                throw new Exception( string.Join(",",resp.Errors));
            }
        }
        

        return true;
    }

    [Queue("default")]
    public async Task<int> SyncScripMasterAsync(CancellationToken cancellationToken)
    {
        var r = new Result<bool>();
        var scripResult = await GrowwService.GetScrips(cancellationToken);

        if (scripResult.HasErrors)
            throw new Exception(scripResult.GetErrors());
        List<EquityStock> updates = [];
        List<EquityStock> creates = [];
        var parh = "D:\\Projects\\Xplor-Inc\\ShareMarket\\Sector.json";
        var content = File.ReadAllText(parh);
        var growwSectors = JsonConvert.DeserializeObject<List<GrowwSectorModel>>(content) ?? [];
        List<GrowwIndustry> industries = [];
        growwSectors.ForEach(s => industries.AddRange(s.Industries));
        var syncFirstTime = !await EquityRepo.FindAll(s => s.IsActive).ResultObject.AnyAsync(cancellationToken: cancellationToken);

        foreach (var item in scripResult.ResultObject)
        {
            string code = item.NseScriptCode ?? item.BseScriptCode ?? string.Empty;

            var equityStock = await EquityRepo.FindAll(s => s.Code == code, asNoTracking:false).ResultObject.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            equityStock ??= new();

            if (updates.Any(x => x.Code == code) || creates.Any(x => x.Code == code))
                continue;

            equityStock.Type            = EquityType.Equity;
            equityStock.Code            = code;
            equityStock.DayChange       = item.LivePriceDto.DayChange;
            equityStock.DayHigh         = item.LivePriceDto.High;
            equityStock.DayLow          = item.LivePriceDto.Low;
            equityStock.LTP             = item.LivePriceDto.Ltp;
            equityStock.Name            = item.CompanyName;
            equityStock.IsActive        = true;
            equityStock.DayChangePer    = item.LivePriceDto.DayChangePerc;
            equityStock.GrowwSearchId   = item.SearchId;
            equityStock.BSECode         = item.BseScriptCode;
            equityStock.Industry        = industries.FirstOrDefault(x=>x.Id == item.IndustryCode)?.Name;
            equityStock.Isin            = item.Isin;
            equityStock.EquityPanditUrl = $"https://www.equitypandit.com/share-price/{item.BseScriptCode ?? item.NseScriptCode}";
            if (equityStock.Id > 0)
                updates.Add(equityStock);
            else if(syncFirstTime)
               creates.Add(equityStock);
        }

        if (updates.Count > 0)
        {
            var equityStockUpdateResult = await EquityRepo.UpdateAsync(updates, SystemConstant.SystemUserId, cancellationToken);
            if (equityStockUpdateResult.HasErrors)
                throw new Exception(equityStockUpdateResult.GetErrors());
        }
        if (creates.Count > 0 && syncFirstTime)
        {
            var equityStockCreateResult = await EquityRepo.CreateAsync(creates, SystemConstant.SystemUserId, cancellationToken);
            if (equityStockCreateResult.HasErrors)
                throw new Exception(equityStockCreateResult.GetErrors());
        }
        return creates.Count + updates.Count;
    }

    [Queue("ltp")]
    public async Task<int> SyncEquityLTPAsync(CancellationToken cancellationToken)
    {
        var r = new Result<bool>();
        var date = DateOnly.FromDateTime(DateTime.Now);
        //if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
        //    throw new Exception($"Job can be schedule on week days only from Groww service.");
        //if(DateTime.Now.Hour <10 || DateTime.Now.Hour >20)
        //    throw new Exception($"Job can be schedule on week days only from Groww 10AM to 8PM.");

        var equities = await EquityRepo.FindAll(s => s.IsActive, orderBy: o => o.OrderBy("GrowwRank", "DESC"))
                                       .ResultObject.Select(s => s.Code).ToListAsync(cancellationToken);

        for (int count = 0; count < equities.Count; count++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            var equity = equities[count];
            BackgroundJob.Enqueue<EquityDailyPriceSyncConductor>(x => x.SyncLtpByStockAsync(equity, date, $"{count+1}/{equities.Count}")); break;
        }

        return equities.Count;
    }

    [Queue("default")]
    public async Task<int> SyncSchemeAsync(CancellationToken cancellationToken)
    {
        var schemeResult = await GrowwService.GetSchemes(cancellationToken);

        if (schemeResult.HasErrors)
            throw new Exception(schemeResult.GetErrors());
        
        List<Scheme> updates = [];
        List<Scheme> creates = [];
        ;
        foreach (var schemeModel in schemeResult.ResultObject)
        {
            if (cancellationToken.IsCancellationRequested) break;
            if (updates.Any(x => x.SchemeCode == schemeModel.SchemeCode) || creates.Any(x => x.SchemeCode == schemeModel.SchemeCode))
                continue;

            var scheme = await SchemeRepo.FindAll(s => s.SchemeCode == schemeModel.SchemeCode).ResultObject.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            scheme ??= new Scheme();

            scheme.SchemeName       = schemeModel.SchemeName;
            scheme.SchemeCode       = schemeModel.SchemeCode;
            scheme.Category         = schemeModel.Category;
            scheme.DirectSchemeName = schemeModel.DirectSchemeName;
            scheme.DocType          = schemeModel.DocType;
            scheme.FundHouse        = schemeModel.FundHouse;
            scheme.FundName         = schemeModel.FundName;
            scheme.GrowwRating      = schemeModel.GrowwRating;
            scheme.LaunchDate       = schemeModel.LaunchDate;
            scheme.Risk             = schemeModel.Risk;
            scheme.RiskRating       = schemeModel.RiskRating;
            scheme.SchemeType       = schemeModel.SchemeType;
            scheme.SearchId         = schemeModel.SearchId;
            scheme.SubCategory      = schemeModel.SubCategory;

            if (scheme.Id > 0)
                updates.Add(scheme);
            else
                creates.Add(scheme);
        }
        if (updates.Count > 0)
        {
            var schemeUpdateResult = await SchemeRepo.UpdateAsync(updates, SystemConstant.SystemUserId, cancellationToken);
            if (schemeUpdateResult.HasErrors)
                throw new Exception(schemeUpdateResult.GetErrors());
        }
        if (creates.Count > 0)
        {
            var schemeCreateResult = await SchemeRepo.CreateAsync(creates, SystemConstant.SystemUserId, cancellationToken);
            if (schemeCreateResult.HasErrors)
                throw new Exception(schemeCreateResult.GetErrors());
        }
        var schemes = await SchemeRepo.FindAll(s => s.DeletedOn == null).ResultObject.ToListAsync(cancellationToken: cancellationToken);
        foreach (var item in schemes)
        {
            BackgroundJob.Enqueue<IEquityDailyPriceSyncConductor>(x => x.SyncSchemeHoldingsAsync(item));
        }
        BackgroundJob.Schedule<IEquityDailyPriceSyncConductor>(x => x.UpdateRank(), TimeSpan.FromMinutes(30));
        return schemeResult.ResultObject.Count;
    }

    [Queue("default")]
    public async Task<int> SyncSchemeHoldingsAsync(Scheme scheme)
    {
        //List<SchemeHoldingModel> NoSearchIds = [];
        //List<SchemeHoldingModel> NoCodes = [];

        var r = new Result<bool>();

        var schemeResult = await GrowwService.GetSchemeHoldings(scheme);

        if (schemeResult.HasErrors || schemeResult.ResultObject is null)
            throw new Exception(schemeResult.GetErrors());

        List<SchemeEquityHolding> updates = [];
        List<SchemeEquityHolding> creates = [];

        List<string> sIds = schemeResult.ResultObject.Holdings.Where(e => e.StockSearchId != null).Select(s => s.StockSearchId ?? string.Empty).ToList();
        var codes = await EquityRepo.FindAll(x => x.IsActive && x.GrowwSearchId != null && sIds.Contains(x.GrowwSearchId)).ResultObject.ToListAsync();
        foreach (var x in schemeResult.ResultObject.Holdings)
        {
            if(x.StockSearchId is null || x.CompanyName is null)
            {
                //NoSearchIds.Add(x);
                continue;
            }
            var hasCode = codes.FirstOrDefault(a => a.GrowwSearchId == x.StockSearchId);
            if (hasCode == null)
            {
               // NoCodes.Add(x);
                continue;
            }
            var holding = await SchemeHoldingRepo.FindAll(s => s.StockSearchId == x.StockSearchId && s.SchemeCode == x.SchemeCode).ResultObject.FirstOrDefaultAsync();
            holding ??= new SchemeEquityHolding();

            holding.EquityId        = hasCode.Id;
            holding.Code            = hasCode.Code;
            holding.SchemeId        = scheme.Id;
            holding.SchemeCode      = scheme.SchemeCode;
            holding.SectorName      = x.SectorName;
            holding.CompanyName     = x.CompanyName;
            holding.CorpusPer       = x.CorpusPer;
            holding.InstrumentName  = x.InstrumentName;
            holding.MarketCap       = x.MarketCap;
            holding.MarketValue     = x.MarketValue;
            holding.NatureName      = x.NatureName;
            holding.Rating          = x.Rating;
            holding.RatingMarketCap = x.RatingMarketCap;
            holding.StockSearchId   = x.StockSearchId;
            
            if (holding.Id > 0)
                updates.Add(holding);
            else
                creates.Add(holding);
        }
      
        if (updates.Count > 0)
        {
            var schemeHoldingUpdateResult = await SchemeHoldingRepo.UpdateAsync(updates, SystemConstant.SystemUserId);
            if (schemeHoldingUpdateResult.HasErrors)
                throw new Exception(schemeHoldingUpdateResult.GetErrors());
        }
        if (creates.Count > 0)
        {
            var schemeHoldingCreateResult = await SchemeHoldingRepo.CreateAsync(creates, SystemConstant.SystemUserId);
            if (schemeHoldingCreateResult.HasErrors)
                throw new Exception(schemeHoldingCreateResult.GetErrors());
        }
        //if (NoSearchIds.Count > 0)
        //{
        //    string path_NoSearchIds = $"D:\\Projects\\NoSearchIds\\{scheme.SchemeCode}.json";
        //    string content_NoSearchIds = JsonConvert.SerializeObject(NoSearchIds);
        //    File.WriteAllText(path_NoSearchIds, content_NoSearchIds);
        //}
        //if (NoCodes.Count > 0)
        //{
        //    string path = $"D:\\Projects\\NoCodes\\{scheme.SchemeCode}.json";
        //    string content = JsonConvert.SerializeObject(NoCodes);
        //    File.WriteAllText(path, content);
        //}
        return schemeResult.ResultObject.Holdings.Count;
    }

    [Queue("ltp")]
    public async Task<string> SyncLtpByStockAsync(string code, DateOnly date, string loop)
    {
        var equity = await EquityRepo.FindAll(s => s.IsActive && s.Code == code)
                    .ResultObject.FirstOrDefaultAsync() ?? throw new Exception($"Equity Stock data not found with code: {code}");

        var ltpPriceResult = await GrowwService.GetFnOPrice("MIDCPNIFTY24DEC12750PE");
        if (ltpPriceResult.HasErrors)
            throw new Exception(ltpPriceResult.GetErrors());

        var Ltp = ltpPriceResult.ResultObject;
        if (Ltp != null && Ltp.Ltp == 0)
        {
            throw new Exception($"Equity Stock LTP found as 0 from Groww with code: {code}");
        }
        if (Ltp is null)
            throw new Exception($"Equity Stock LTP found as NULL from Groww with code: {code}");

        equity.LTP          = Ltp.Ltp;
        equity.LTPDate      = date;
        equity.DayChange    = Ltp.DayChange;
        equity.DayChangePer = Ltp.DayChangePerc;
        equity.DayHigh      = Ltp.High;
        equity.DayLow       = Ltp.Low;

        var equityUpdateResult = await EquityRepo.UpdateAsync(equity, SystemConstant.SystemUserId);
        if (equityUpdateResult.HasErrors)
            throw new Exception(equityUpdateResult.GetErrors());

        var history = new EquityPriceHistory
        {
            Close       = Ltp.Ltp,
            Code        = equity.Code,
            Date        = date,
            EquityId    = equity.Id,
            Low         = Ltp.Low,
            Name        = equity.Name,
            High        = Ltp.High,
            Open        = Ltp.Open,
        };

        var createResult = await TechnicalCalulationConductor.CreateOrUpdateHistoryAsync([history]);
        if (createResult.HasErrors)
            throw new Exception(createResult.GetErrors());
        await NotificationHub.Groww($"Syncing {loop}", $"{equity.Code}");
        var scheduledJobs = JobStorage.Current.GetMonitoringApi().GetStatistics();
        if (scheduledJobs.Processing <= 1 && scheduledJobs.Enqueued <= 1)
            BackgroundJob.Enqueue<IEquityTechnicalCalulationConductor>(x => x.SyncEquietyStockCalculation(equity,""));
        return $"{code}:{date:dd-MMM-yyyy}:{equity.LTP}";
    }

    [Queue("history")]
    public async Task<int> SyncHistoryByPandit(CancellationToken cancellationToken)
    {
        var equities = await EquityRepo.FindAll(s => s.DeletedOn == null).ResultObject.ToListAsync(cancellationToken: cancellationToken);
        int count = 0;
        foreach (var equity in equities)
        {
            count++;
            BackgroundJob.Enqueue<IEquityDailyPriceSyncConductor>(x => x.SyncHistoryByPandit(equity, $"{count}/{equities.Count}"));
        }
        return equities.Count;
    }

    [Queue("history")]
    public async Task<int> SyncHistoryByPandit(EquityStock equity, string loop)
    {
        var historyResult = await EquityPanditService.SyncPriceEquityPandit(equity);
        if (historyResult.HasErrors)
            throw new Exception(historyResult.GetErrors());
        var histories = historyResult.ResultObject;
        var ifAny = await HistoryRepo.FindAll(x => x.DeletedOn == null && x.Code == equity.Code).ResultObject.AnyAsync();
        if(!ifAny)
        {
            var createResult = await HistoryRepo.CreateAsync(histories, SystemConstant.SystemUserId);
            if (createResult.HasErrors)
                throw new Exception(createResult.GetErrors());
        }
        else
        {
            var createResult = await TechnicalCalulationConductor.CreateOrUpdateHistoryAsync(histories);
            if (createResult.HasErrors)
                throw new Exception(createResult.GetErrors());
        }
       
        await NotificationHub.EquityPandit($"Syncing {loop}", $"{equity.Code}");
        await NotificationHub.WatchlistSync($"WatchlistSync", $"{loop}");

        var scheduledJobs = JobStorage.Current.GetMonitoringApi().GetStatistics();
        if (scheduledJobs.Processing <= 1 && scheduledJobs.Enqueued <= 1)
        {
            //BackgroundJob.Enqueue<EquityDailyPriceSyncConductor>(x => x.SyncScripMasterAsync(CancellationToken.None));
            //BackgroundJob.Schedule<EquityDailyPriceSyncConductor>(x => x.RSI_X_EMA_DMA_Calculation(CancellationToken.None), TimeSpan.FromSeconds(5));
        }

        return histories.Count;
    }

    [Queue("default")]
    public async Task<int> EnqueFundamentalSyncByPandit(CancellationToken cancellationToken)
    {
        var equities = await EquityRepo.FindAll(s => s.IsActive).ResultObject.ToListAsync(cancellationToken: cancellationToken);
        foreach (var equity in equities)
        {
            BackgroundJob.Enqueue<EquityDailyPriceSyncConductor>(x => x.FundamentalSyncByPandit(equity,""));
        }
        return equities.Count;
    }

    [Queue("default")]
    public async Task<bool> FundamentalSyncByPandit(EquityStock equity,string message = "")
    {
        var fundamentalResult = await EquityPanditService.SyncFundamentalEquityPandit(equity);
        //if (fundamentalResult.HasErrors)
        //    throw new Exception(fundamentalResult.GetErrors());
        if(string.IsNullOrEmpty(message))
        await NotificationHub.EquityPandit("FundamentalSync", $"{equity.Code}:-Pandit-:{equity.LTP}");
        else
            await NotificationHub.WatchlistSync("WatchlistSync", $"{message}");

        var updateResult = await EquityRepo.UpdateAsync(fundamentalResult.ResultObject, SystemConstant.SystemUserId);
        if (updateResult.HasErrors)
            throw new Exception(updateResult.GetErrors());
        return true;
    }

    [Queue("calculation")]
    public async Task<int> RSI_X_EMA_DMA_Calculation(CancellationToken cancellationToken)
    {
        var equities = await EquityRepo.FindAll(s => s.IsActive).ResultObject.ToListAsync(cancellationToken: cancellationToken);
        foreach (var equity in equities)
        {
            BackgroundJob.Enqueue<IEquityTechnicalCalulationConductor>(x => x.SyncEquietyStockCalculation(equity,""));
        }
        return equities.Count;
    }

    [Queue("default")]
    public async Task<int> SyncShareHoldingByScreener(CancellationToken cancellationToken)
    {
        var equities = await EquityRepo.FindAll(s => s.IsActive && s.PromotersHolding == 0).ResultObject.ToListAsync(cancellationToken: cancellationToken);
        foreach (var equity in equities)
        {
            if (cancellationToken.IsCancellationRequested) break;
            await NotificationHub.Screener("SyncShareHoldingByScreener", $"{equity.Code}:-Sleeping...", cancellationToken);

            await Task.Delay(1000 * 5, cancellationToken);
            await NotificationHub.Screener("SyncShareHoldingByScreener", $"{equity.Code}:-Syncing.....", cancellationToken);
            var shareHolding = await ScreenerService.SyncShareHoldingByScreener(equity);
            if (shareHolding > 0)
            {
                equity.PromotersHolding = shareHolding;
                await NotificationHub.Screener("SyncShareHoldingByScreener", $"{equity.Code}:-Upating...", cancellationToken);
                var updateResult = await EquityRepo.UpdateAsync(equity, SystemConstant.SystemUserId, cancellationToken);
                if (updateResult.HasErrors)
                {
                    await NotificationHub.Screener("SyncShareHoldingByScreener", $"{equity.Code}:-Error... {updateResult.GetErrors()}", cancellationToken);
                }
            }
            await NotificationHub.Screener("SyncShareHoldingByScreener", $"{equity.Code}:- Process Completed...", cancellationToken);
        }
        return equities.Count;

    }
    
    [Queue("default")]
    public async Task<decimal> SyncShareHoldingByScreener(EquityStock equity)
    {
        var shareHolding = await ScreenerService.SyncShareHoldingByScreener(equity);
        if (shareHolding > 0)
        {
            await NotificationHub.Screener("Share Holding", $"{equity.Code}:- Process Completed...");
            equity.PromotersHolding = shareHolding;
            var updateResult = await EquityRepo.UpdateAsync(equity, SystemConstant.SystemUserId);
            if (updateResult.HasErrors)
            {
                await NotificationHub.Screener("Share Holding", $"{equity.Code}:-Error... {updateResult.GetErrors()}");
                throw new Exception(updateResult.GetErrors());
            }
            await NotificationHub.Screener("Share Holding", $"{equity.Code}:- Process Completed...");
        }
        return shareHolding;
    }

    [Queue("default")]
    public async Task<int> ProcessETFs(CancellationToken cancellationToken = default)
    {
        var etfs = await GrowwService.GetETFs(cancellationToken);
        List<EquityStock> updates = [];
        List<EquityStock> creates = [];
        var parh = "D:\\Projects\\Xplor-Inc\\ShareMarket\\Sector.json";
        var content = File.ReadAllText(parh);
        var growwSectors = JsonConvert.DeserializeObject<List<GrowwSectorModel>>(content) ?? [];
        List<GrowwIndustry> industries = [];
        growwSectors.ForEach(s => industries.AddRange(s.Industries));
        foreach (var item in etfs.ResultObject)
        {
            string code = item.CompanyHeader.NseScriptCode ?? item.CompanyHeader.BseScriptCode ?? string.Empty;

            var equityStock = await EquityRepo.FindAll(s => s.Code == code, asNoTracking: false).ResultObject.FirstOrDefaultAsync(cancellationToken: cancellationToken);
            equityStock ??= new();

            if (updates.Any(x => x.Code == code) || creates.Any(x => x.Code == code))
                continue;

            equityStock.MarketCap       = item.Aum;
            equityStock.Type            = EquityType.ETF;
            equityStock.Code            = code;
            equityStock.Name            = item.CompanyHeader.ShortName;
            equityStock.IsActive        = true;
            equityStock.GrowwSearchId   = item.CompanyHeader.SearchId;
            equityStock.BSECode         = item.CompanyHeader.BseScriptCode;
            equityStock.Industry        = industries.FirstOrDefault(x => x.Id == item.CompanyHeader.IndustryId)?.Name;
            equityStock.Isin            = item.CompanyHeader.Isin;
            equityStock.EquityPanditUrl = $"https://www.equitypandit.com/share-price/{item.CompanyHeader.BseScriptCode ?? item.CompanyHeader.NseScriptCode}";
            if (equityStock.Id > 0)
                updates.Add(equityStock);
            else 
                creates.Add(equityStock);
        }
        if (updates.Count > 0)
        {
            var equityStockUpdateResult = await EquityRepo.UpdateAsync(updates, SystemConstant.SystemUserId, cancellationToken);
            if (equityStockUpdateResult.HasErrors)
                throw new Exception(equityStockUpdateResult.GetErrors());
        }
        if (creates.Count > 0)
        {
            var equityStockCreateResult = await EquityRepo.CreateAsync(creates, SystemConstant.SystemUserId, cancellationToken);
            if (equityStockCreateResult.HasErrors)
                throw new Exception(equityStockCreateResult.GetErrors());
        }
        return etfs.ResultObject.Count;
    }
}
