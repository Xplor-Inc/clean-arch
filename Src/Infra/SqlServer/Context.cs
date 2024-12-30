using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShareMarket.Core.Interfaces.SqlServer;

namespace ShareMarket.SqlServer;
public abstract class Context(string connectionString, ILoggerFactory? loggerFactory) : DbContext, IContext
{
    #region Overrides

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (loggerFactory != null)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
        }

        optionsBuilder.UseSqlServer(connectionString: connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Remove cascade delete
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Configure mappings
        ConfigureMappings(modelBuilder);

        // Call the base method
        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region IDataContext Implementation
    public new void Add<T>(T entity) where T : class => base.Add(entity);

    public virtual void CreateStructure()
    {
    }

    public virtual void DeleteDatabase()
    {
    }
    public void Delete<T>(T entity) where T : class
    {
        var set = base.Set<T>();
        set.Remove(entity);
    }

    public void DetectChanges() => base.ChangeTracker.DetectChanges();
    public virtual void DropStructure()
    {

    }
    public async Task<int> ExecuteCommandAsync(string command, CancellationToken cancellationToken = default)
    {
        return await base.Database.ExecuteSqlRawAsync(command, cancellationToken);
    }
    public IQueryable<T> Query<T>() where T : class => base.Set<T>();
    public new void Update<T>(T entity) where T : class
    {
        var set = base.Set<T>();

        if (!set.Local.Any(e => e == entity))
        {
            set.Attach(entity);
            SetAsModified(entity);
        }
    }

    #endregion

    #region Public Methods

    public virtual void ConfigureMappings(ModelBuilder modelBuilder)
    {
    }

    #endregion

    #region Private Methods
    private EntityEntry<T> GetEntityEntry<T>(T entity) where T : class
    {
        var entry = Entry(entity);

        if (entry.State == EntityState.Deleted)
        {
            Set<T>().Attach(entity);
        }

        return entry;
    }
    private void SetAsModified<T>(T entity) where T : class
    {
        UpdateEntityState(entity, EntityState.Modified);
    }
    private void UpdateEntityState<T>(T entity, EntityState state) where T : class
    {
        GetEntityEntry(entity).State = state;
    }

    #endregion
}