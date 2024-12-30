using ShareMarket.Core.Models.Configurations;

namespace ShareMarket.SqlServer;
public class ShareMarketConnection : Connection
{
    #region Overrides of Connection

    public override string ToString(string delimiter = ";")
    {
        return $"Data Source={Datasource}; Database={Database}; User Id={UserId}; Password={Password}; {AdditionalParameters}";
    }

    #endregion
}