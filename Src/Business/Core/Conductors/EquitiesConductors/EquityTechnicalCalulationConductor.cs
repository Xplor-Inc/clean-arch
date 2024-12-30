using Hangfire;
using Microsoft.EntityFrameworkCore;
using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Interfaces.Hubs;

namespace ShareMarket.Core.Conductors.EquitiesConductors;

public class EquityTechnicalCalulationConductor(
    INotificationHub                                NotificationHub,
    IRepositoryConductor<EquityStockCalculation>    EquityStockCalculationRepo,
    IRepositoryConductor<EquityPriceHistory>        HistoryRepo) : IEquityTechnicalCalulationConductor
{
    public async Task<Result<bool>> CreateOrUpdateHistoryAsync(List<EquityPriceHistory> histories)
    {
        var r = new Result<bool>();
        foreach (var item in histories)
        {
            var x = await HistoryRepo.FindAll(x => x.Code == item.Code && x.Date == item.Date,asNoTracking:false).ResultObject.FirstOrDefaultAsync();
            if (x != null)
            {
                x.Open          = item.Open;
                x.Close         = item.Close;
                x.High          = item.High;
                x.Low           = item.Low;
                x.DayChange     = item.DayChange;
                x.DayChangePer  = item.DayChangePer;

                var updateResult = await HistoryRepo.UpdateAsync(x, SystemConstant.SystemUserId);
                r.AddErrors(updateResult.Errors);
            }
            else
            {
                var createResult = await HistoryRepo.CreateAsync(item, SystemConstant.SystemUserId);
                r.AddErrors(createResult.Errors);
            }
        }
        return r;
    }

    [Queue("calculation")]
    public async Task<Result<bool>> SyncEquietyStockCalculation(EquityStock EquityStock,string message = "")
    {
        if (string.IsNullOrEmpty(message))
            await NotificationHub.Calculations("RSI_X_EMA_DMA", $"{EquityStock.Code}:- Process Started...");
        else
            await NotificationHub.WatchlistSync("WatchlistSync", message);

        var r = new Result<bool>();
        int days = 14;
        var EqityStockCalculation = await EquityStockCalculationRepo.FindAll(x => x.EquityStockId== EquityStock.Id,asNoTracking:false).ResultObject.FirstOrDefaultAsync();
        EqityStockCalculation ??= new EquityStockCalculation();

        if (EqityStockCalculation is null) return r;
        var histories = await HistoryRepo.FindAll(x => x.Code == EquityStock.Code,asNoTracking:false, orderBy: o => o.OrderBy("Date", "ASC")).ResultObject.ToListAsync();
        if (histories.Count == 0) return r;
        var lastYear = DateOnly.FromDateTime(DateTime.Now).AddYears(-1);
        for (int i = 0; i < histories.Count; i++)
        {
            if (i == 0) continue;
            #region RSI Calculation
            
            var x0 = histories[i - 1];
            var x1 = histories[i];
            var profit = x1.Close - x0.Close;
            var loss = x0.Close - x1.Close;
            if (profit < 0) profit = 0;
            if (loss < 0) loss = 0;
            histories[i].Profit = profit;
            histories[i].Loss = loss;
            if (i > (days - 1))
            {
                if (i == days)
                {
                    var gain14Days = histories.Skip(1).Take(days).Average(s => s.Profit);

                    var loss14Days = histories.Skip(1).Take(days).Average(s => s.Loss);

                    histories[i].Avg14DaysLoss = Math.Round(loss14Days, 2, MidpointRounding.AwayFromZero);
                    histories[i].Avg14DaysProfit = Math.Round(gain14Days, 2, MidpointRounding.AwayFromZero);
                    if (loss14Days > 0)
                        histories[i].RS = Math.Round(gain14Days / loss14Days, 2, MidpointRounding.AwayFromZero);
                    histories[i].RSI = Math.Round(100 - (100 / (1 + histories[i].RS)), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    var sum0 = Math.Round(((histories[i - 1].Avg14DaysProfit * (days - 1)) + histories[i].Profit) / days, 2, MidpointRounding.AwayFromZero);
                    var loss0 = Math.Round(((histories[i - 1].Avg14DaysLoss * (days - 1)) + histories[i].Loss) / days, 2, MidpointRounding.AwayFromZero);
                    histories[i].Avg14DaysLoss = loss0;
                    histories[i].Avg14DaysProfit = sum0;
                    if (loss0 > 0)
                        histories[i].RS = Math.Round(sum0 / loss0, 2, MidpointRounding.AwayFromZero);
                    histories[i].RSI = Math.Round(100 - (100 / (1 + histories[i].RS)), 2, MidpointRounding.AwayFromZero);
                }
            }
            #endregion

            #region DMA Calculation 5,10,20,50,100,200
            if (i > 4)
                histories[i].DMA5 = histories.Skip(i - 4).Take(5).Average(s => s.Close);
            if (i > 8)
                histories[i].DMA10 = histories.Skip(i - 9).Take(10).Average(s => s.Close);

            if (i > 18)
                histories[i].DMA20 = histories.Skip(i - 19).Take(20).Average(s => s.Close);

            if (i > 48)
                histories[i].DMA50 = histories.Skip(i - 49).Take(50).Average(s => s.Close);

            if (i > 98)
                histories[i].DMA100 = histories.Skip(i - 99).Take(100).Average(s => s.Close);

            if (i > 198)
                histories[i].DMA200 = histories.Skip(i - 199).Take(200).Average(s => s.Close);

            #endregion
           
            #region RSI14EMADiff
            if (i < 27) continue;

            if (i == 27)
            {
                var rSI14DMA = histories.Skip(days).Take(days).Average(a => a.RSI);
                histories[i].RSI14EMA = Math.Round(rSI14DMA, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                histories[i].RSI14EMA = Math.Round(((histories[i - 1].RSI14EMA * (days - 1)) + histories[i].RSI) / days, 2, MidpointRounding.AwayFromZero);
                histories[i].RSI14EMADiff = histories[i].RSI14EMA - histories[i - 1].RSI14EMA;
            } 
            #endregion
        }
        var yH = histories.Where(x => x.Date >= lastYear).OrderByDescending(O => O.High).FirstOrDefault();
        var yL = histories.Where(x => x.Date >= lastYear).OrderBy(O => O.Low).FirstOrDefault();
        var last = histories.Last();
        
        EqityStockCalculation.RSI14EMA      = last.RSI14EMA;
        EqityStockCalculation.RSI14EMADiff  = last.RSI14EMADiff;
        EqityStockCalculation.RSI           = last.RSI;
        EqityStockCalculation.DMA5          = last.DMA5;
        EqityStockCalculation.DMA10         = last.DMA10;
        EqityStockCalculation.DMA20         = last.DMA20;
        EqityStockCalculation.DMA50         = last.DMA50;
        EqityStockCalculation.DMA100        = last.DMA100;
        EqityStockCalculation.DMA200        = last.DMA200;
        
        if (yH is not null)
        {
            EqityStockCalculation.YearHigh = yH.High;
            EqityStockCalculation.YearHighOn = yH.Date;
        }
        if (yL is not null)
        {
            EqityStockCalculation.YearLow = yL.Low;
            EqityStockCalculation.YearLowOn = yL.Date;
        }
        if (yH is not null && yL is not null)
            EqityStockCalculation.IsRaising = EqityStockCalculation.YearLowOn > EqityStockCalculation.YearHighOn;

        #region Stock Growth
        EqityStockCalculation.PerChange_1W = CalculatePercent(histories, DateOnly.FromDateTime(DateTime.Now.AddDays(-8)),   EquityStock);
        EqityStockCalculation.PerChange_15D = CalculatePercent(histories, DateOnly.FromDateTime(DateTime.Now.AddDays(-15)), EquityStock);
        EqityStockCalculation.PerChange_1M = CalculatePercent(histories, DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)), EquityStock);
        EqityStockCalculation.PerChange_3M = CalculatePercent(histories, DateOnly.FromDateTime(DateTime.Now.AddMonths(-3)), EquityStock);
        EqityStockCalculation.PerChange_6M = CalculatePercent(histories, DateOnly.FromDateTime(DateTime.Now.AddMonths(-6)), EquityStock);
        EqityStockCalculation.PerChange_1Y = CalculatePercent(histories, DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),  EquityStock);
        #endregion


        var historyUpdateResult = await HistoryRepo.UpdateAsync(histories, SystemConstant.SystemUserId);
        if (EqityStockCalculation.Id > 0)
        {
            EqityStockCalculation.EquityStock = null;
            var resp = await EquityStockCalculationRepo.UpdateAsync(EqityStockCalculation, SystemConstant.SystemUserId);
            r.AddErrors(resp.Errors);
        }
        else
        {
            EqityStockCalculation.EquityStockId = EquityStock.Id;
            var resp = await EquityStockCalculationRepo.CreateAsync(EqityStockCalculation, SystemConstant.SystemUserId);
            r.AddErrors(resp.Errors);
        }

        r.AddErrors(historyUpdateResult.Errors);

        if (r.HasErrors)
            throw new Exception(r.GetErrors());

        if (string.IsNullOrEmpty(message))
            await NotificationHub.Calculations("RSI_X_EMA_DMA", $"{EquityStock.Code}:- Process Completed...");
        else
            await NotificationHub.SendMessage("WatchlistSync", message);

        return r;
    }


    private static decimal CalculatePercent(List<EquityPriceHistory> hist, DateOnly dt, EquityStock EqityStock)
    {
        try
        {
            var DayHistory = hist.OrderByDescending(x => x.Date).LastOrDefault(x => x.Date >= dt);
            if (DayHistory == null)
                return 0;
            var per = ((EqityStock.LTP - DayHistory.Close) / DayHistory.Close) * 100;
            return Math.Round(per, 1);
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
