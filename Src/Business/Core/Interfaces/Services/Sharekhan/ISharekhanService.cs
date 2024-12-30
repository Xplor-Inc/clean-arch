using ShareMarket.Core.Models.Sharekhan;

namespace ShareMarket.Core.Interfaces.Services.Sharekhan;

public interface ISharekhanService
{
    Task<Result<string>> GenerateAccessToken(string redirectUrl, string requestToken);

    Task<Result<SharekhanProfileData>> HistoricalData(string apiKey, string accessToken, string exchange, int scripCode, string interval = "Day");

    Task<Result<SharekhanScripMasterData>> ScriptMaster(string apiKey, string accessToken, string exchange);

}
