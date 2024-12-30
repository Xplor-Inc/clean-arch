using ShareMarket.Core.Entities;
using ShareMarket.Core.Interfaces.SqlServer;
using System.Linq.Expressions;

namespace ShareMarket.Core.Conductors;
public class RepositoryConductor<T>(IRepository<T> Repository) : IRepositoryConductor<T>  where T : Entity
{
    public virtual Task<Result<T>> CreateAsync(T item, long createdById, CancellationToken cancellationToken = default) => Repository.CreateAsync(item, createdById, cancellationToken);
    public virtual Task<Result<List<T>>> CreateAsync(IEnumerable<T> items, long createdById, CancellationToken cancellationToken = default) => Repository.CreateAsync(items, createdById, cancellationToken);
    public virtual Task<Result<bool>> UpdateAsync(T item, long updatedBy, CancellationToken cancellationToken = default) => Repository.UpdateAsync(item, updatedBy, cancellationToken);
    public virtual Task<Result<bool>> UpdateAsync(IEnumerable<T> items, long updatedBy, CancellationToken cancellationToken = default) => Repository.UpdateAsync(items, updatedBy, cancellationToken);
    public virtual Task<Result<bool>> DeleteAsync(long id, long deletedById, bool soft = true, CancellationToken cancellationToken = default) => Repository.DeleteAsync(id, deletedById, soft, cancellationToken);
    public virtual Task<Result<bool>> DeleteAsync(T o, long deletedById, bool soft = true, CancellationToken cancellationToken = default) => Repository.DeleteAsync(o, deletedById, soft, cancellationToken);
    public virtual Task<Result<int>> ExecuteCommandAsync(string commandText, CancellationToken cancellationToken = default) => Repository.ExecuteCommandAsync(commandText, cancellationToken);
    public virtual Result<IQueryable<T>> FindAll(
        Expression<Func<T, bool>>?                  filter              = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>?  orderBy             = null,
        string?                                     includeProperties   = null,
        int?                                        skip                = default,
        int?                                        take                = default,
        bool?                                       ignoreQueryFilters  = false,
        bool                                        asNoTracking        = true
    ) => Repository.FindAll(filter, orderBy, includeProperties, skip, take, ignoreQueryFilters, asNoTracking);

}