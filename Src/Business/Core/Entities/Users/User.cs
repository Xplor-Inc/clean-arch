using ShareMarket.Core.Enumerations;

namespace ShareMarket.Core.Entities.Users;

public class User : Auditable
{
    #region Properties
    public string           EmailAddress        { get; set; } = string.Empty;
    public string           FirstName           { get; set; } = string.Empty;
    public bool             IsActivated         { get; set; }
    public bool             IsActive            { get; set; }
    public DateTimeOffset?  LastLoginDate       { get; set; }
    public string           LastName            { get; set; } = string.Empty;
    public DateTimeOffset?  PasswordChangeDate  { get; set; }
    public string           PasswordHash        { get; set; } = string.Empty;
    public string           PasswordSalt        { get; set; } = string.Empty;
    public UserRole         Role                { get; set; }
    public string           SecurityStamp       { get; set; } = string.Empty;

    #endregion Properties

    #region Navigation Properties

    public virtual List<User>? UsersCreatedBy { get; set; }
    public virtual List<User>? UsersDeletedBy { get; set; }
    public virtual List<User>? UsersUpdatedBy { get; set; }
    #endregion

}
