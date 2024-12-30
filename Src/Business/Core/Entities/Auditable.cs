using ShareMarket.Core.Entities.Users;

namespace ShareMarket.Core.Entities;

public class Auditable : Entity
{
    #region Properties
    public long?            UpdatedById { get; set; }
    public DateTimeOffset?  UpdatedOn   { get; set; }
    public long?            DeletedById { get; set; }
    public DateTimeOffset?  DeletedOn   { get; set; }

    #endregion

    #region Navigation Properties
    public User?            UpdatedBy   { get; set; }
    public User?            DeletedBy   { get; set; }
    #endregion
}
