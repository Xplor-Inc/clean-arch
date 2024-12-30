using System.Linq.Expressions;
using ShareMarket.Core.Interfaces.SqlServer;
using ShareMarket.Core.Models.Results;

namespace ShareMarket.SqlServer.Repositories;
public class Repository<T>(IContext context, ILogger<Repository<T>> logger) : IRepository<T>
    where T : Entity
{
    #region Properties
    public IContext                 Context { get; private set; }   = context;
    public ILogger<Repository<T>>   Logger  { get; }                = logger;
    public IQueryable<T>            Query   { get; private set; }   = context.Query<T>();

    private static readonly char[] separator = [','];

    #endregion

    #region IRepository Implementation
    public virtual async Task<Result<int>> ExecuteCommandAsync(string commandText, CancellationToken cancellationToken)
    {
        var result = new Result<int>(0);

        try
        {
            result.ResultObject = await Context.ExecuteCommandAsync(commandText, cancellationToken);
        }
        catch (Exception ex)
        {
            result.Errors = HandleException(ex);
        }
        return result;
    }

    public virtual async Task<Result<T>> CreateAsync(T entity, long createdById, CancellationToken cancellationToken = default)
    {
        var result = new Result<T>(entity);

        try
        {
            entity.CreatedOn    = DateTimeOffset.Now.ToIst();
            entity.CreatedById  = createdById;

            Context.Add(entity);
            await Context.SaveChangesAsync(cancellationToken);

            result.ResultObject = entity;
        }
        catch (Exception ex)
        {
            result.Errors = HandleException(ex);
        }

        return result;
    }

    public virtual async Task<Result<List<T>>> CreateAsync(IEnumerable<T> entities, long createdById, CancellationToken cancellationToken = default)
    {
        var result = new Result<List<T>>([]);

        try
        {
            var numInserted = 0;

            foreach (var entity in entities)
            {
                entity.CreatedOn   = DateTimeOffset.Now.ToIst(); 
                entity.CreatedById = createdById;

                Context.Add(entity);
                result.ResultObject.Add(entity);

                if (++numInserted >= 100)
                {
                    numInserted = 0;
                    await Context.SaveChangesAsync(cancellationToken);
                }
            }

            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            result.Errors = HandleException(ex);
        }

        return result;
    }

    public virtual async Task<Result<bool>> DeleteAsync(long id, long deletedById, bool soft = true, CancellationToken cancellationToken = default)
    {
        Result<bool> findResult = default!;
       var xfindResult = FindAll(x => x.Id == id);
        if (xfindResult.HasErrors)
        {
            return new Result<bool>(false)
            {
                Errors = xfindResult.Errors
            };
        }
        var entity = await xfindResult.ResultObject.FirstOrDefaultAsync(cancellationToken);
        if(entity is null)
        {
            findResult.AddError($"Entity not found with Id: {id}");
            return findResult;
        }
        return await DeleteAsync(entity, deletedById, soft, cancellationToken);
    }

    public virtual async Task<Result<bool>> DeleteAsync(T entity, long deletedById, bool soft = true, CancellationToken cancellationToken = default)
    {
        var result = new Result<bool>(false);

        try
        {
            if (entity == null)
            {
                result.AddError($"{entity?.GetType()} does not exist.");
                return result;
            }

            if (soft)
            {
                if(entity is Auditable auditable)
                {
                    auditable.DeletedById   = deletedById;
                    auditable.DeletedOn     = DateTimeOffset.Now.ToIst();
                }
                else
                {
                    result.AddError($"{entity.GetType()} is not deleteatable.");
                    return result;
                }
            }
            else
            {
                Context.Delete(entity);
            }

            await Context.SaveChangesAsync(cancellationToken);
            result.ResultObject = true;
        }
        catch (Exception ex)
        {
            result.Errors = HandleException(ex);
        }

        return result;
    }

    public virtual Result<IQueryable<T>> FindAll(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null,
        int? skip = null, int? take = null, bool? ignoreQueryFilters = false, bool asNoTracking = true)
    {
        var result = new Result<IQueryable<T>>(default!);

        try
        {
            result.ResultObject = GetQueryable(filter, orderBy, includeProperties, skip, take, ignoreQueryFilters, asNoTracking);
        }
        catch (Exception ex)
        {
            result.Errors = HandleException(ex);
        }

        return result;
    }

    public virtual async Task<Result<bool>> UpdateAsync(T entity, long updatedBy, CancellationToken cancellationToken = default)
    {
        var result = new Result<bool>(false);

        try
        {
            if (entity is Auditable auditable)
            {
                auditable.UpdatedById   = updatedBy;
                auditable.UpdatedOn     = DateTimeOffset.Now.ToIst();
            }
            else
            {
                result.AddError($"{entity.GetType()} is not auditable.");
                return result;
            }
            Context.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);

            result.ResultObject = true;
        }
        catch (Exception ex)
        {
            result.Errors = HandleException(ex);
        }

        return result;
    }

    public virtual async Task<Result<bool>> UpdateAsync(IEnumerable<T> entities, long updatedBy, CancellationToken cancellationToken = default)
    {
        var result = new Result<bool>(false);
        try
        {
            var counts = 0;
            foreach (var entity in entities)
            {
                counts++;
                if (entity is Auditable auditable)
                {
                    auditable.UpdatedById   = updatedBy;
                    auditable.UpdatedOn     = DateTimeOffset.Now.ToIst();
                }
                else
                {
                    result.AddError($"{entity.GetType()} is not auditable.");
                    return result;
                }
                Context.Update(entity);
                if (counts == 100)
                {
                    await Context.SaveChangesAsync(cancellationToken);
                    counts = 0;
                }
            }
            await Context.SaveChangesAsync(cancellationToken);
            result.ResultObject = true;
        }
        catch (Exception ex)
        {
            result.Errors = HandleException(ex);
        }
        result.ResultObject = true;
        return result;
    }

    #endregion

    #region Protected Methods

    public virtual IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null,
        int? skip = null, int? take = null, bool? ignoreQueryFilters = false, bool asNoTracking = true)
    {
        includeProperties ??= string.Empty;
        var query         = Query;

        if (ignoreQueryFilters.HasValue && ignoreQueryFilters.Value)
        {
            query = query.IgnoreQueryFilters();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        foreach (var includeProperty in includeProperties.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    #endregion

    #region Private Method
    private List<string> HandleException(Exception ex)
    {
        var returnError = new List<string>
        {
           $"Exception : {ex.Message}"
        };
        if (ex.InnerException != null)
            returnError.Add($"\nInnerException : {ex.InnerException.Message}");

        var loggerError = $"Exception : {ex.Message} \n {ex.StackTrace}";
        if (ex.InnerException != null)
            loggerError += $"\nInnerException : {ex.InnerException.Message} \n {ex.InnerException.StackTrace}";

        Logger.LogCritical("{loggerError}", loggerError);
        return returnError;
    }

    #endregion
}
