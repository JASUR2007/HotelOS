namespace HotelOS.PaymentService.Models;

public sealed class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Open";
    public DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddMinutes(10);
}

public sealed class Payment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTimeOffset ProcessedAt { get; set; }
}

public sealed class TransactionLog
{
    public int Id { get; set; }
    public int PaymentId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class PaymentHistory
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset ChangedAt { get; set; }
}

public sealed class IdempotentRefund
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public int PaymentId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}