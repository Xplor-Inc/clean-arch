﻿using Microsoft.Extensions.Logging;
using ShareMarket.Core.Extensions;
using ShareMarket.Core.Models.Results;

namespace ShareMarket.Core.Utilities;
public class Do<T>(Func<Result<T>, T> workload)
{
    #region Properties

    public Exception?           Exception   { get; set; }
    public Result<T>            Result      { get; private set; } = new Result<T>(default!);
    public Func<Result<T>, T>   Workload    { get; } = workload;

    #endregion Properties


    #region Public Methods

    public Do<T> Catch<TException>(Action<TException, Result<T>> handler)
        where TException : Exception
    {
        if (Exception == null)
        {
            return this;
        }

        if (Exception.GetType() == typeof(TException)
            || typeof(TException) == typeof(Exception))
        {
            handler((TException)Exception, Result);
        }

        return this;
    }
    public Do<T> Finally(ILogger logger, Action<Result<T>> workload)
    {
        try
        {
            workload(Result);
        }
        catch (Exception ex)
        {
            Result.AddExceptionError(ex);
            logger.LogError(ex, "exception");
        }

        return this;
    }
    public Do<T> Finally(Action<Result<T>> workload) 
    {
        try
        {
            workload(Result);
        }
        catch (Exception ex)
        {
            Result.AddExceptionError(ex);
        }

        return this;
    }
    public static Do<T> Try(ILogger logger, Func<Result<T>, T> workload)
    {
        var d = new Do<T>(workload);

        try
        {
            d.Result.ResultObject = workload(d.Result);
        }
        catch (Exception ex)
        {
            d.Result.AddExceptionError(ex);
            d.Exception = ex;
            logger.LogError(exception: ex, "Exception");
        }

        return d;
    }
    public static Do<T> Try(Func<Result<T>, T> workload)
    {
        var d = new Do<T>(workload);

        try
        {
            d.Result.ResultObject = workload(d.Result);
        }
        catch (Exception ex)
        {
            d.Result.AddExceptionError(ex);
            d.Exception = ex;
        }

        return d;
    }
    public static Do<T> Try(ILogger logger, uint retry, Func<Result<T>, T> workload)
    {
        if (retry == 0)
        {
            return Try(logger, workload);
        }

        var attempts = 0;
        var d = new Do<T>(workload);

        while (attempts != retry)
        {
            d = Try(logger, workload);

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