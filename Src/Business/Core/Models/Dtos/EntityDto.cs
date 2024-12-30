using ShareMarket.Core.Models.Dtos.Users;

namespace ShareMarket.Core.Models.Dtos;

public class EntityDto
{   
    #region Properties
    public long             Id          { get; set; }
    public long             CreatedById { get; set; }
    public DateTimeOffset   CreatedOn   { get; set; }
    #endregion

    #region Navigation Properties
    public UserDto          CreatedBy   { get; set; } = default!;
    #endregion

}
