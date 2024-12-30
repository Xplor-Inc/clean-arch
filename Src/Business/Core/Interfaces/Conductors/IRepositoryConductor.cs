using ShareMarket.Core.Entities;
using System.Linq.Expressions;

namespace ShareMarket.Core.Interfaces.Conductors;
public interface IRepositoryConductor<T>   where T : Entity
{
    Task<Result<T>>             CreateAsync(T item, long createdById, CancellationToken cancellationToken = default);
    Task<Result<List<T>>>       CreateAsync(IEnumerable<T> items, long createdById, CancellationToken cancellationToken = default);
    Task<Result<bool>>          DeleteAsync(long id, long deletedById, bool soft = true, CancellationToken cancellationToken = default);
    Task<Result<bool>>          DeleteAsync(T o, long deletedById, bool soft = true, CancellationToken cancellationToken = default);
    Result<IQueryable<T>>       FindAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null, int? skip = null, int? take = null, bool? ignoreQueryFilters = false, bool asNoTracking = true);
    Task<Result<bool>>          UpdateAsync(T item, long updatedBy, CancellationToken cancellationToken = default);
    Task<Result<bool>>          UpdateAsync(IEnumerable<T> items, long updatedBy, CancellationToken cancellationToken = default);
    Task<Result<int>>           ExecuteCommandAsync(string commandText, CancellationToken cancellationToken = default);
}