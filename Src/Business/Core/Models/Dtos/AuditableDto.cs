using ShareMarket.Core.Models.Dtos.Users;

namespace ShareMarket.Core.Models.Dtos;

public class AuditableDto : EntityDto
{
    #region Properties
    public long?            UpdatedById { get; set; }
    public DateTimeOffset?  UpdatedOn   { get; set; }
    public long?            DeletedById { get; set; }
    public DateTimeOffset?  DeletedOn   { get; set; }

    #endregion

    #region Navigation Properties
    public UserDto?            UpdatedBy   { get; set; }
    public UserDto?            DeletedBy   { get; set; }
    #endregion
}
