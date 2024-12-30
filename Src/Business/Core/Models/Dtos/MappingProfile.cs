using AutoMapper;
using ShareMarket.Core.Entities;
using ShareMarket.Core.Entities.Equities;
using ShareMarket.Core.Entities.Tradings;
using ShareMarket.Core.Entities.Users;
using ShareMarket.Core.Models.Dtos.Equities;
using ShareMarket.Core.Models.Dtos.Trading;
using ShareMarket.Core.Models.Dtos.Users;

namespace ShareMarket.Core.Models.Dtos;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User,                     UserDto>().ReverseMap();
        CreateMap<EquityStock,              EquityStockDto>().ReverseMap();
        CreateMap<EquityPriceHistory,       EquityPriceHistoryDto>().ReverseMap();
        CreateMap<TradeBook,                TradeBookDto>().ReverseMap();
        CreateMap<Watchlist,                WatchlistDto>().ReverseMap();
        CreateMap<WatchlistStock,           WatchlistStockDto>().ReverseMap();
        CreateMap<EquityStockCalculation,   EquityStockCalculationDto>().ReverseMap();
        CreateMap<TradeBook,                TradeOrder>().ReverseMap();
        CreateMap<TradeOrder,               TradeOrderDto>().ReverseMap();
        CreateMap<TradeOrderDto,            TradeOrderDto>();
    }
}
