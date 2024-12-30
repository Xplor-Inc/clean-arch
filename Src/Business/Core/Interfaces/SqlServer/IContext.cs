namespace ShareMarket.Core.Interfaces.SqlServer;
public interface IContext : IDisposable
{
    void                Add<T>(T entity) where T : class;
    void                CreateStructure();
    void                DeleteDatabase();
    void                Delete<T>(T entity) where T : class;
    void                DropStructure();
    IQueryable<T>       Query<T>() where T : class;
    Task<int>           SaveChangesAsync(CancellationToken cancellationToken = default);    
    Task<int>           ExecuteCommandAsync(string command, CancellationToken cancellationToken = default);
    void                Update<T>(T entity) where T : class;
}