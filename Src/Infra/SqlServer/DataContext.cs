using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ShareMarket.Core.Interfaces.SqlServer;

namespace ShareMarket.SqlServer
{
    public class DataContext<TUser>(string connectionString, ILoggerFactory? loggerFactory) 
        : Context(connectionString, loggerFactory), IDataContext<TUser>
        where TUser : User
    {
        #region Properties

        public DbSet<ChangeLog>     ChangeLogs  { get; set; }
        public DbSet<User>          Users       { get; set; }

        #endregion Properties


        #region IDataContext Implementation

        IQueryable<User>    IDataContext<TUser>.Users  => Users;

        #endregion IDataContext Implementation

        #region Overrides of DataContext

        public override void ConfigureMappings(ModelBuilder modelBuilder)
        {
            base.ConfigureMappings(modelBuilder);
        }

        public override void CreateStructure()
        {
            Database.Migrate();
        }

        public override void DeleteDatabase()
        {
            Database.EnsureDeleted();
        }

        public override void DropStructure()
        {
            var migrator = this.GetInfrastructure().GetRequiredService<IMigrator>();
            migrator.Migrate("0");
        }
        public override int SaveChanges()
        {
            try
            {
                var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
                long actionBy = 0;
                foreach (var change in modifiedEntities)
                {
                    string entityName = change.Entity.GetType().Name;
                    long primaryKey = ((Entity)change.Entity).Id;
                    var changeLogs = new List<ChangeLog>();
                    var UpdatedById = change.CurrentValues["UpdatedById"];
                    var CreatedById = change.CurrentValues["CreatedById"];
                    if (UpdatedById != null)
                    {
                        actionBy = (long)UpdatedById;
                    }
                    if (actionBy == 0 && CreatedById != null)
                    {
                        actionBy = (long)CreatedById;
                    }

                    foreach (var prop in change.OriginalValues.Properties)
                    {
                        if (IgnoredProperties.Contains(prop.Name)) continue;

                        var originalValue = change.OriginalValues[prop]?.ToString();
                        var currentValue = change.CurrentValues[prop]?.ToString();
                        if (prop.ClrType.Name == "Int64")
                        {
                            if(long.TryParse(originalValue, out var old) && long.TryParse(currentValue, out var current))
                            {
                                if (old == current) continue;
                            }
                        }
                        if (prop.ClrType.Name == "Decimal")
                        {
                            if (decimal.TryParse(originalValue, out var old) && decimal.TryParse(currentValue, out var current))
                            {
                                if (old == current) continue;
                            }
                        }
                        if (originalValue != currentValue) //Only create a log if the value changes
                        {
                            changeLogs.Add(new ChangeLog()
                            {
                                EntityName      = entityName,
                                PrimaryKey      = primaryKey,
                                PropertyName    = prop.Name,
                                OldValue        = originalValue,
                                NewValue        = currentValue,
                                CreatedOn       = DateTimeOffset.Now.ToIst(),
                                CreatedById     = actionBy
                            });
                        }
                    }
                    ChangeLogs.AddRange(changeLogs);
                }
            }
            catch (Exception)
            {
            }
            return base.SaveChanges();
        }

        public string[] IgnoredProperties => ["CreatedById", "DeletedOn", "DeletedById", "Id", "UpdatedOn"];


        #endregion Overrides of DataContext
    }
}
