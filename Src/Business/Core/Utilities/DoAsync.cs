using Microsoft.Extensions.Logging;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Models.Results;

namespace ShareMarket.Core.Utilities;

public class DoAsync<T>
{
    #region Properties

    public Exception? Exception { get; set; }
    public Result<T> Result { get; private set; } = new Result<T>(default!);
    public Func<Result<T>, Task<T>> Workload { get; }

    #endregion Properties

    #region Constructor
    public DoAsync(Func<Result<T>, Task<T>> workload)
    {
        Workload = workload;
    }
    #endregion Constructor

    #region Public Methods

    public async Task<DoAsync<T>> CatchAsync<TException>(Func<TException, Result<T>, Task> handler)
        where TException : Exception
    {
        if (Exception == null)
        {
            return this;
        }
        if (Exception.GetType() == typeof(TException) || typeof(TException) == typeof(Exception))
        {
            await handler((TException)Exception, Result);
        }

        return this;
    }

    public async Task<DoAsync<T>> FinallyAsync(ILogger logger, Func<Result<T>, Task> workload)
    {
        try
        {
            await workload(Result);
        }
        catch (Exception ex)
        {
            Result.AddExceptionError(ex);
            logger.LogError(ex, "Exception during Finally block");
        }

        return this;
    }

    public async Task<DoAsync<T>> FinallyAsync(Func<Result<T>, Task> workload)
    {
        try
        {
            await workload(Result);
        }
        catch (Exception ex)
        {
            Result.AddExceptionError(ex);
        }

        return this;
    }

    public static async Task<DoAsync<T>> TryAsync(ILogger logger, Func<Result<T>, Task<T>> workload)
    {
        var d = new DoAsync<T>(workload);

        try
        {
            d.Result.ResultObject = await workload(d.Result);
        }
        catch (Exception ex)
        {
            d.Result.AddExceptionError(ex);
            d.Exception = ex;
            logger.LogError(ex, "Exception during Try block");
        }

        return d;
    }

    public static async Task<DoAsync<T>> TryAsync(Func<Result<T>, Task<T>> workload)
    {
        var d = new DoAsync<T>(workload);

        try
        {
            d.Result.ResultObject = await workload(d.Result);
        }
        catch (Exception ex)
        {
            d.Result.AddExceptionError(ex);
            d.Exception = ex;
        }

        return d;
    }

    public static async Task<DoAsync<T>> TryAsync(ILogger logger, uint retry, Func<Result<T>, Task<T>> workload)
    {
        if (retry == 0)
        {
            return await TryAsync(logger, workload);
        }

        var attempts = 0;
        var d = new DoAsync<T>(workload);

        while (attempts != retry)
        {
            d = await TryAsync(logger, workload);

            if (!d.Result.HasErrors)
            {
                break;
            }

            attempts++;
        }

        return d;
    }

    #endregion Public Methods
}
