using HotelOS.Shared.Audit;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;

namespace HotelOS.Shared.RabbitMQ;

public sealed class RabbitMqAuditLogger(IEventPublisher publisher) : IAuditLogger
{
    public void Log(string actor, string action, string entity, string details = "")
    {
        publisher.Publish("audit.logged", new AuditEntry(actor, action, entity, details, DateTimeOffset.UtcNow));
    }
}
