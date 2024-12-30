using ShareMarket.Core.Models.Results;

namespace ShareMarket.Core.Extensions;
public static class ResultExtensions
{
    public static string AddError<T>(this Result<T> result, string error)
    {
        result.Errors ??= [];

        result.Errors.Add(error);

        return error;
    }
    public static string AddExceptionError<T>(this Result<T> result, Exception exception)
    {
        var message = $"Exception : {exception.Message}\n{exception.StackTrace}";
        if (exception.InnerException != null)
            message += $"InnerException : {exception.InnerException.Message}\n{exception.InnerException.StackTrace}";
        return result.AddError(message);
    }
    public static List<string>? AddErrors<T>(this Result<T> result, IEnumerable<string> errors)
    {
        if (!errors.Any())
        {
            return result.Errors;
        }

        foreach (var error in errors)
        {
            result.AddError(error);
        }

        return result.Errors;
    }

    public static string GetErrors<T>(this Result<T> result, string delimiter = "\n")
    {
        if (!result.HasErrors || result.Errors is null)
        {
            return string.Empty;
        }

        delimiter = string.IsNullOrEmpty(delimiter) ? "\n" : delimiter;

        return string.Join(delimiter, result.Errors);
    }
}
