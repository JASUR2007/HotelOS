namespace HotelOS.PaymentService.Events;

public sealed record InvoiceGeneratedEvent(int InvoiceId, string InvoiceNumber, DateTimeOffset OccurredAt);
public sealed record PaymentCompletedEvent(int PaymentId, int InvoiceId, decimal Amount, DateTimeOffset OccurredAt);
public sealed record RefundProcessedEvent(int PaymentId, string Reason, DateTimeOffset OccurredAt);