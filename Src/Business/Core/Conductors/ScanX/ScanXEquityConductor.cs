using Hangfire;
using Microsoft.EntityFrameworkCore;
using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Entities.ScanX;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Interfaces.Conductors.ScanX;
using ShareMarket.Core.Interfaces.Services.ScanX;
using ShareMarket.Core.Services.ScanX;

namespace ShareMarket.Core.Conductors.ScanX;

public class ScanXEquityConductor(
    IGrowwService                               GrowwService,
    IRepositoryConductor<ScanXEquity>           ScanXRepo,
    IRepositoryConductor<EquityStock>           EquityRepo,
    IRepositoryConductor<ScanXIndicator>        ScanXIndicatorRepo,
    IRepositoryConductor<ScanXIndicatorColumn>  ScanXIndicatorColumnRepo,
    IScanXService ScanXService) : IScanXEquityConductor
{
    public async Task GetAllStocks()
    {
        var ty = await ScanXService.GetScrips(CancellationToken.None);
        List<ScanXEquity> equities = ty.ResultObject;
        foreach (var equity in equities)
        {
            BackgroundJob.Enqueue<IScanXEquityConductor>(x => x.CreateOrUpdate(equity));
        }
    }

    public async Task CreateOrUpdate(ScanXEquity equity)
    {
         var stock = await EquityRepo.FindAll(x => x.Isin == equity.Isin).ResultObject.FirstOrDefaultAsync();
         if (stock == null)
         {
             var isValifByGroww = await GrowwService.GetLTPPrice(equity.Sym);
            if (!isValifByGroww.HasErrors)
            {
                stock = new EquityStock
                {
                    Code            = equity.Sym,
                    DayChange       = equity.PPerchange,
                    LTP             = equity.Ltp,
                    Name            = equity.DispSym,
                    IsActive        = true,
                    DayChangePer    = equity.PPerchange,
                    GrowwSearchId   = equity.Seosym,
                    Industry        = equity.SubSector,
                    Sector          = equity.Sector
                };
                var createResult = await EquityRepo.CreateAsync(stock, SystemConstant.SystemUserId);
                if (createResult.HasErrors && !createResult.Errors.Any(a=>a.Contains("Cannot insert duplicate key row in object 'dbo.EquityStocks' with unique index ")))
                {
                    throw new Exception(createResult.GetErrors());
                }
            }
         }
         equity.EquityId = stock?.Id;
         var scanXEntity = await ScanXRepo.FindAll(x => x.Sid == equity.Sid, asNoTracking: false).ResultObject.FirstOrDefaultAsync();
         if (scanXEntity == null)
         {
             var createScanXResult = await ScanXRepo.CreateAsync(equity, SystemConstant.SystemUserId);
             if (createScanXResult.HasErrors)
             {
                 throw new Exception(createScanXResult.GetErrors());
             }
         }
         else
         {
             scanXEntity.CopyFrom(equity);

             var updateScanXResult = await ScanXRepo.UpdateAsync(scanXEntity, SystemConstant.SystemUserId);
             if (updateScanXResult.HasErrors)
             {
                 throw new Exception(updateScanXResult.GetErrors());
             }
         }
    }
    public async Task GetIndicatorData(CancellationToken cancellationToken = default)
    {
        var scanXEquities = await ScanXRepo.FindAll(x => x.DeletedOn == null && x.EquityId.HasValue).ResultObject.ToListAsync(cancellationToken);
        foreach (var scanX in scanXEquities)
        {
            if (cancellationToken.IsCancellationRequested) break;
            BackgroundJob.Enqueue<IScanXEquityConductor>(x => x.GetIndicatorData_Column(scanX, cancellationToken));
        }

    }
    public async Task GetIndicatorData_Column(ScanXEquity scanX, CancellationToken cancellationToken = default)
    {
        var result = await ScanXService.GetIndicators(scanX, cancellationToken);

        List<ScanXIndicatorColumn> indicators = [];
        if (result.HasErrors)
        {
            throw new Exception($"{result.GetErrors()}");
        }

        foreach (var indicator in result.ResultObject.Data)
        {
            if (indicator.Indicator.Count == 0) continue;
            indicators.Add(new ScanXIndicatorColumn
            {
                Action_1    = indicator.Indicator[0].Action,
                Indicator_1 = indicator.Indicator[0].Indicator,
                Value_1     = indicator.Indicator[0].Value,
                Action_2    = indicator.Indicator[1].Action,
                Indicator_2 = indicator.Indicator[1].Indicator,
                Value_2     = indicator.Indicator[1].Value,
                Action_3    = indicator.Indicator[2].Action,
                Indicator_3 = indicator.Indicator[2].Indicator,
                Value_3     = indicator.Indicator[2].Value,
                Action_4    = indicator.Indicator[3].Action,
                Indicator_4 = indicator.Indicator[3].Indicator,
                Value_4     = indicator.Indicator[3].Value,
                Action_5    = indicator.Indicator[4].Action,
                Indicator_5 = indicator.Indicator[4].Indicator,
                Value_5     = indicator.Indicator[4].Value,
                Action_6    = indicator.Indicator[5].Action,
                Indicator_6 = indicator.Indicator[5].Indicator,
                Value_6     = indicator.Indicator[5].Value,
                Action_7    = indicator.Indicator[6].Action,
                Indicator_7 = indicator.Indicator[6].Indicator,
                Value_7     = indicator.Indicator[6].Value,
                Action_8    = indicator.Indicator[7].Action,
                Indicator_8 = indicator.Indicator[7].Indicator,
                Value_8     = indicator.Indicator[7].Value,
                Action_9    = indicator.Indicator[8].Action,
                Indicator_9 = indicator.Indicator[8].Indicator,
                Value_9     = indicator.Indicator[8].Value,
            });
        }
        indicators.ForEach(x => { x.ScanXId = scanX.Id; x.EquityId = scanX.EquityId; });
        var createXIndicatorResult = await ScanXIndicatorColumnRepo.CreateAsync(indicators, SystemConstant.SystemUserId, cancellationToken);
        if (createXIndicatorResult.HasErrors)
        {
            throw new Exception($"{createXIndicatorResult.GetErrors()}");
        }

    }
    public async Task GetIndicatorData(ScanXEquity scanX, CancellationToken cancellationToken = default)
    {
        var result = await ScanXService.GetIndicators(scanX, cancellationToken);

        List<ScanXIndicator> indicators = [];
        if (result.HasErrors)
        {
            throw new Exception($"{result.GetErrors()}");
        }

        foreach (var indicator in result.ResultObject.Data)
        {
            foreach (var item1 in indicator.Indicator)
            {
                FillIndicators("Indicator", indicators, item1);
            }
            foreach (var item1 in indicator.EMA)
            {
                FillIndicators("EMA", indicators, item1);
            }
            foreach (var item1 in indicator.SMA)
            {
                FillIndicators("SMA", indicators, item1);
            }
            foreach (var item1 in indicator.Pivot)
            {
                FillSupportRegistance("Range", "Fibonacci", indicators, item1.Fibonacci);
                FillSupportRegistance("Range", "Camarilla", indicators, item1.Camarilla);
                FillSupportRegistance("Range", "Classic", indicators, item1.Classic);
            }
        }
        indicators.ForEach(x => { x.ScanXId = scanX.Id; x.EquityId = scanX.EquityId; });
        var createXIndicatorResult = await ScanXIndicatorRepo.CreateAsync(indicators, SystemConstant.SystemUserId, cancellationToken);
        if (createXIndicatorResult.HasErrors)
        {
            throw new Exception($"{createXIndicatorResult.GetErrors()}");
        }
    }
    private static void FillSupportRegistance(string group, string Name, List<ScanXIndicator> indicators, PivotData item)
    {
        indicators.Add(new ScanXIndicator
        {
            Action = Name,
            Indicator = "PP",
            Value = item.PP,
            Group = "PP"
        });
        indicators.Add(new ScanXIndicator
        {
            Action = Name,
            Indicator = "R1",
            Value = item.R1,
            Group = "Resistance"
        });
        indicators.Add(new ScanXIndicator
        {
            Action = Name,
            Indicator = "R2",
            Value = item.R2,
            Group = "Resistance"
        });
        indicators.Add(new ScanXIndicator
        {
            Action = Name,
            Indicator = "R3",
            Value = item.R3,
            Group = "Resistance"
        });
        indicators.Add(new ScanXIndicator
        {
            Action = Name,
            Indicator = "S1",
            Value = item.S1,
            Group = "Support"
        });
        indicators.Add(new ScanXIndicator
        {
            Action = Name,
            Indicator = "S2",
            Value = item.S2,
            Group = "Support"
        });
        indicators.Add(new ScanXIndicator
        {
            Action = Name,
            Indicator = "S3",
            Value = item.S3,
            Group = "Support"
        });

       
    }
    private static void FillIndicators(string group, List<ScanXIndicator> indicators, IndicatorValue indicator)
    {
        indicators.Add(new ScanXIndicator
        {
            Action = indicator.Action,
            Indicator = indicator.Indicator,
            Value = indicator.Value,
            Group = group
        });
    }

}
