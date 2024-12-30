using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using ShareMarket.Core.Entities.Settings;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Interfaces.Services.Sharekhan;
using ShareMarket.Core.Models.Sharekhan;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShareMarket.Core.Services.Sharekhan;

public partial class SharekhanService(IRepositoryConductor<AppConfig> AppConfigRepo) : ISharekhanService
{
    private readonly HttpClient Client = new();
    private readonly string     ApiUrl = "https://api.sharekhan.com/skapi/services";
  
    public async Task<Result<string>> GenerateAccessToken(string redirectUrl, string requestToken)
    {
        Result<string> result = new(default!);
        try
        {
            requestToken = requestToken.Replace(" ", "+");
            var x = await AppConfigRepo.FindAll(x => x.Provider == "Sharekhan" && x.RedirectUrl.StartsWith(redirectUrl) && x.DeletedOn == null)
                                        .ResultObject.FirstOrDefaultAsync();
            if (x is null)
            {
                result.AddError("Provider is not configured");
                return result;
            }
            
            string decData = DecryptStringFromAES(requestToken, x.ApiSecretKey);
            string[] tokenParts = decData.Split('|');
            string newToken = tokenParts[1] + "|" + tokenParts[0];
            string encData = EncryptStringToAES(newToken, x.ApiSecretKey);

            var json = JsonSerializer.Serialize(new
            {
                apiKey = x.ApiKey,
                requestToken = encData,
            });

            string url = ApiUrl + "/access/token";

            var resultx = await GetToken<SharekhanTokenResult>(url, json);
            if (resultx.HasErrors)
            {
                result.Errors = resultx.Errors;
                return result;
            }
            if (resultx == null || resultx.ResultObject == null || resultx.ResultObject.Data.Token is null)
            {
                result.AddError("Error while getting error");
                return result;
            }

            result.ResultObject = resultx.ResultObject.Data.Token;

            return result;
        }
        catch (Exception ex)
        {
            result.AddExceptionError(ex);
            return result;
        }
    }
    
    public async Task<Result<SharekhanProfileData>> HistoricalData(string apiKey, string accessToken, string exchange, int scripCode, string interval = "Day")
    {
        string uRL = ApiUrl + "/historical/" + exchange + "/" + scripCode + "/" + interval;
        var data = await GetResponse<SharekhanProfileData>(apiKey, accessToken, uRL);
        return data;
    }
    
    public async Task<Result<SharekhanScripMasterData>> ScriptMaster(string apiKey, string accessToken, string exchange)
    {
        string uRL = ApiUrl + $"/master/{exchange}";
        return await GetResponse< SharekhanScripMasterData>(apiKey, accessToken, uRL);
    }
    
    private async Task<Result<T>> GetResponse<T>(string apiKey, string accessToken, string URL)
    {
        Result<T> result = new(default!);
        try
        {
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("access-token", accessToken);
            Client.DefaultRequestHeaders.Add("api-key", apiKey);

            HttpResponseMessage response = await Client.GetAsync(URL);

            if (response.IsSuccessStatusCode)
            {
                result.ResultObject = await response.Content.ReadAsAsync<T>();
            }
            return result;
        }
        catch (Exception ex)
        {
            result.AddExceptionError(ex);
            return result;
        }
    }
    
    private async Task<Result<T>> GetToken<T>(string url, string data)
    {
        var result = new Result<T>();
        try
        {
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                result.ResultObject = await response.Content.ReadAsAsync<T>();
                var xz = await response.Content.ReadAsStringAsync();
            }
            else
                result.AddError(await response.Content.ReadAsStringAsync());
            return result;
        }
        catch (Exception ex)
        {
            result.AddExceptionError(ex);
            return result;
        }
    }

    private static string DecryptStringFromAES(string encryptedText, string key)
    {
        string iv = "AAAAAAAAAAAAAAAAAAAAAA==";
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Convert.FromBase64String(iv);

        string base64EncryptedText = encryptedText.Replace('-', '+').Replace('_', '/').PadRight(encryptedText.Length + (4 - encryptedText.Length % 4) % 4, '=');

        byte[] encryptedBytes = Convert.FromBase64String(base64EncryptedText);

        GcmBlockCipher cipher = new(new AesEngine());
        AeadParameters parameters = new(new KeyParameter(keyBytes), 128, ivBytes, null);
        cipher.Init(false, parameters);

        byte[] decryptedBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
        int len = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, decryptedBytes, 0);
        cipher.DoFinal(decryptedBytes, len);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private static string EncryptStringToAES(string plaintext, string key)
    {
        string iv = "AAAAAAAAAAAAAAAAAAAAAA==";
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Convert.FromBase64String(iv);
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        GcmBlockCipher cipher = new (new AesEngine());
        AeadParameters parameters = new (new KeyParameter(keyBytes), 128, ivBytes, null);
        cipher.Init(true, parameters);

        byte[] encryptedBytes = new byte[cipher.GetOutputSize(plaintextBytes.Length)];
        int len = cipher.ProcessBytes(plaintextBytes, 0, plaintextBytes.Length, encryptedBytes, 0);
        cipher.DoFinal(encryptedBytes, len);

        return Convert.ToBase64String(encryptedBytes);
    }
}