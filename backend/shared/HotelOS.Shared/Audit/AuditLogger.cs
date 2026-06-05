namespace HotelOS.Shared.Audit;

public sealed record AuditEntry(
    string Actor,
    string Action,
    string Entity,
    string Details,
    DateTimeOffset OccurredAt);

public interface IAuditLogger
{
    void Log(string actor, string action, string entity, string details = "");
}
