namespace ShareMarket.Core.Entities.Audits;

public class EmailAuditLog : Auditable
{
    public string?  Attachments     { get; set; }
    public string?  CCEmails        { get; set; }
    public string?  Error           { get; set; }
    public string   HeaderText      { get; set; } = default!;
    public string   MessageBody     { get; set; } = default!;
    public string   Subject         { get; set; } = default!;
    public bool     Success         { get; set; }
    public string   ToEmails        { get; set; } = default!;
}
