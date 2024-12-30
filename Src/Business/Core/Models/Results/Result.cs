using ShareMarket.Core.Extensions;

namespace ShareMarket.Core.Models.Results;

public class Result<T>
{
    #region Properties    
    public virtual List<string>               Errors         { get; set; } = [];
    public virtual bool                       HasErrors      => Errors != null && Errors.Count != 0;
    public virtual T                          ResultObject   { get; set; }
    public long                               RowCount       { get; set; }
    public Dictionary<string, object>?        Info           { get; set; }    
    
    #endregion Properties
    
    
    #region Constructors
    
    public Result(T resultObject, string errorMessage)  { ResultObject = resultObject; this.AddError(errorMessage); }
    public Result(T resultObject)                       => ResultObject = resultObject;
    public Result()                                     => ResultObject = default!;
    #endregion Constructors
}
