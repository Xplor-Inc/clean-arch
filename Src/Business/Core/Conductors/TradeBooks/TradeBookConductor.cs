using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShareMarket.Core.Entities.Tradings;
using ShareMarket.Core.Enumerations;
using ShareMarket.Core.Extensions;

namespace ShareMarket.Core.Conductors.TradeBooks;

public class TradeBookConductor(
    IMapper                                 Mapper,
    ILogger<TradeBookConductor>             Logger,
    IRepositoryConductor<TradeOrder>        TradeBookOrderRepo,
    IRepositoryConductor<TradeBook>         TradeBookRepo,
    IGrowwService                           GrowwService) : ITradeBookConductor
{
    public async Task<Result<TradeOrder>> CreateTradeBookOrderAsync(TradeOrder order, long createdbyId, CancellationToken cancellationToken = default)
    {
        var r = new Result<TradeOrder>(order);

        var stockServiceResult = await GrowwService.GetLTPPrice(order.Code, cancellationToken);
        if (stockServiceResult.HasErrors)
        {
            r.AddErrors(stockServiceResult.Errors);
            return r;
        }
        var liveData = stockServiceResult.ResultObject;
        if (liveData is null)
        {
            r.AddError($"Live rates not found for stock {order.Code} from Groww service");
            return r;
        }

        var tradeBook = await TradeBookRepo.FindAll(x => x.EquityId == order.EquityId && x.Strategy == order.Strategy && x.TradeType == order.TradeType
                                                         && x.TradingAccount == order.TradingAccount && x.CreatedById == createdbyId,
                                                asNoTracking: false)
                                   .ResultObject.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        bool aggrigateBook = tradeBook != null;
        if (tradeBook is null)
        {
            tradeBook = Mapper.Map<TradeBook>(order);
            tradeBook.Postion       = order.Quantity;
            tradeBook.BuyDate       = order.OrderDate;
            tradeBook.BuyRate       = order.OrderRate;
            tradeBook.BuyValue      = order.OrderValue;
            tradeBook.MarginAmount  = tradeBook.GetMarginAmount;
            tradeBook.TargetRate    = tradeBook.BuyRate + (tradeBook.BuyRate * tradeBook.TargetPerc / 100);
            var createBookResult = await TradeBookRepo.CreateAsync(tradeBook, createdbyId, cancellationToken);
            if (createBookResult.HasErrors)
            {
                r.AddErrors(createBookResult.Errors);
                await TradeBookOrderRepo.DeleteAsync(order, createdbyId, false, cancellationToken);
            }
        }

        order.OrderValue    = order.OrderRate * order.Quantity;
        order.DailyMftInt   = (order.MarginAmount * SystemConstant.GROWW_MFT_RATE).ToFixed();
        order.TradeBookId   = tradeBook.Id;
        order.HoldingDays = 1;
        var createResult = await TradeBookOrderRepo.CreateAsync(order, createdbyId, cancellationToken);
        if (createResult.HasErrors)
        {
            r.AddErrors(createResult.Errors);
            return r;
        }

        if (aggrigateBook)
        {
            await AggregateTradeBookAsync(tradeBook.Id, createdbyId, cancellationToken);
        }
        return r;
    }

    public async Task<Result<bool>> AggregateTradeBookAsync(long bookId, long createdbyId, CancellationToken cancellationToken = default)
    {
        var r = new Result<bool>(false);

        var tradeBook = await TradeBookRepo.FindAll(x => x.Id == bookId, asNoTracking: false, includeProperties:"Orders")
                                   .ResultObject.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        bool AggrigateBook = tradeBook != null;
        if (tradeBook is null)
        {
            r.AddError($"Book not found with Id: {bookId}");
            return r;
        }
        var stockServiceResult = await GrowwService.GetLTPPrice(tradeBook.Code, cancellationToken);
        if (stockServiceResult.HasErrors)
        {
            r.AddErrors(stockServiceResult.Errors);
            return r;
        }
        var liveData = stockServiceResult.ResultObject;
        if (liveData is null)
        {
            r.AddError($"Live rates not found for stock {tradeBook.Code} from Groww service");
            return r;
        }
        var buyOrders = tradeBook.Orders.FindAll(x => x.OrderType == OrderType.Buy && x.DeletedOn == null);
        var sellOrders = tradeBook.Orders.FindAll(x => x.OrderType == OrderType.Sell && x.DeletedOn == null);
        var bQ = buyOrders.Sum(x => x.Quantity);
        var sQ = sellOrders.Sum(x => x.Quantity);
        
        tradeBook.Postion       = bQ - sQ;
        tradeBook.Quantity      = bQ;
        tradeBook.BuyRate       = buyOrders.Sum(x => x.Quantity * x.OrderRate) / bQ;
        tradeBook.BuyValue      = tradeBook.Quantity * tradeBook.BuyRate;
        tradeBook.MarginAmount  = tradeBook.GetMarginAmount;
        tradeBook.TargetPerc    = buyOrders.Average(s => s.TargetPerc);
        tradeBook.TargetRate    = tradeBook.BuyRate + (tradeBook.BuyRate * tradeBook.TargetPerc / 100);
        if (sQ > 0)
        {
            tradeBook.SellRate      = sellOrders.Sum(x => x.Quantity * x.OrderRate) / sQ;
            tradeBook.ReleasedPL    = (tradeBook.SellRate -tradeBook.BuyRate) * sQ;
        }
        if (tradeBook.Postion == 0 && sellOrders.Count > 0)
            tradeBook.SellDate = sellOrders.Max(m => m.OrderDate);
        if (tradeBook.SellDate is not null)
        {
            tradeBook.HoldingDays = tradeBook.GetHoldingDays;
            tradeBook.ReleasedPLPerc = (tradeBook.ReleasedPL / tradeBook.BuyValue * 100).ToFixed();
        }

        var updateBookResult = await TradeBookRepo.UpdateAsync(tradeBook, createdbyId, cancellationToken);
        if (updateBookResult.HasErrors)
        {
            throw new Exception(updateBookResult.GetErrors());
        }
        return r;
    }

    public async Task<int> AggregateTradeBookAsync(CancellationToken cancellationToken = default)
    {
        var books = await TradeBookOrderRepo.FindAll(x => x.DeletedOn == null).ResultObject.ToListAsync(cancellationToken);
        foreach (var item in books)
        {
            if (cancellationToken.IsCancellationRequested) return books.Count;
            BackgroundJob.Enqueue<ITradeBookConductor>(x => x.AggregateTradeBookAsync(item.Id, SystemConstant.SystemUserId, cancellationToken));
        }
        return books.Count;
    }

    public async Task UpdateCurrentRatesAsync(CancellationToken cancellationToken = default)
    {
        var tradeBooks = await TradeBookRepo.FindAll(e => e.DeletedOn == null && e.SellDate == null, asNoTracking: false, includeProperties: "Equity").ResultObject.ToListAsync(cancellationToken);

        foreach (var trade in tradeBooks)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var stockServiceResult = await GrowwService.GetLTPPrice(trade.Code);
            if (stockServiceResult.HasErrors)
            {
                string error = stockServiceResult.GetErrors();
                Logger.LogCritical("{error}", error);
                continue;
            }
            var liveData = stockServiceResult.ResultObject;
            if (liveData is null) continue;
            trade.MarketValue           = liveData.Ltp * trade.Quantity;
            trade.Equity.LTP            = liveData.Ltp;
            trade.Equity.DayChange      = liveData.DayChange;
            trade.Equity.DayChangePer   = liveData.DayChangePerc;
            trade.ReleasedPL            = trade.MarketValue - trade.BuyValue;
            trade.HoldingDays           = trade.GetHoldingDays;
            trade.DailyMftInt           = (trade.MarginAmount * SystemConstant.GROWW_MFT_RATE).ToFixed();
            trade.MarginInterest        = trade.DailyMftInt * trade.HoldingDays;
            var tradeUpdateResult = await TradeBookRepo.UpdateAsync(trade, SystemConstant.SystemUserId, cancellationToken);
            if (tradeUpdateResult.HasErrors)
            {
                string error = tradeUpdateResult.GetErrors();
                Logger.LogCritical("{error}", error);
                continue;
            }

            //var equityUpdateResult = await EquityStockRepo.UpdateAsync(trade.Equity, SystemConstant.SystemUserId, cancellationToken);
            //if (equityUpdateResult.HasErrors)
            //{
            //    string error = equityUpdateResult.GetErrors();
            //    Logger.LogCritical("{error}", error);
            //    continue;
            //}
        }
    }
}

public interface ITradeBookConductor
{
    Task<Result<TradeOrder>> CreateTradeBookOrderAsync(TradeOrder stock, long createdbyId, CancellationToken cancellationToken = default);
    Task UpdateCurrentRatesAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> AggregateTradeBookAsync(long bookId, long createdbyId, CancellationToken cancellationToken = default);
    Task<int> AggregateTradeBookAsync(CancellationToken cancellationToken = default);
}