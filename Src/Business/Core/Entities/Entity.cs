using ShareMarket.Core.Entities.Users;

namespace ShareMarket.Core.Entities;

public class Entity
{
    #region Properties
    public long             Id          { get; set; }
    public long             CreatedById { get; set; }
    public DateTimeOffset   CreatedOn   { get; set; }
    #endregion

    #region Navigation Properties
    public User             CreatedBy   { get; set; } = default!;
    #endregion
}
