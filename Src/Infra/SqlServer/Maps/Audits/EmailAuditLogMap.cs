namespace ShareMarket.SqlServer.Maps.Audits;

public class EmailAuditLogMap : Map<EmailAuditLog>
{
    public override void Configure(EntityTypeBuilder<EmailAuditLog> entity)
    {
        entity
            .ToTable("EmailAuditLogs");

        entity
            .Property(e => e.Attachments)
            .HasMaxLength(StaticConfiguration.MAX_LENGTH);

        entity
            .Property(e => e.CCEmails)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Error)
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.HeaderText)
            .IsRequired()
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);

        entity
            .Property(e => e.ToEmails)
            .IsRequired()
            .HasMaxLength(StaticConfiguration.COMMAN_LENGTH);
    }
}