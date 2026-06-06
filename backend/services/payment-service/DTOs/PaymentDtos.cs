namespace HotelOS.PaymentService.DTOs;

public sealed record CreateInvoiceDto
{
    public string InvoiceNumber { get; init; }
    public string GuestName { get; init; }
    public string RoomNumber { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal RoomNightsTotal { get; init; }
    public decimal FoodOrdersTotal { get; init; }
    public decimal MinibarTotal { get; init; }
    public decimal DamagesTotal { get; init; }
    public decimal DiscountsTotal { get; init; }

    public CreateInvoiceDto(string invoiceNumber, string guestName, string roomNumber, decimal totalAmount,
        decimal roomNightsTotal = 0, decimal foodOrdersTotal = 0, decimal minibarTotal = 0, decimal damagesTotal = 0, decimal discountsTotal = 0)
    {
        InvoiceNumber = invoiceNumber;
        GuestName = guestName;
        RoomNumber = roomNumber;
        TotalAmount = totalAmount;
        RoomNightsTotal = roomNightsTotal;
        FoodOrdersTotal = foodOrdersTotal;
        MinibarTotal = minibarTotal;
        DamagesTotal = damagesTotal;
        DiscountsTotal = discountsTotal;
    }
}

public sealed record ProcessPaymentDto
{
    public int InvoiceId { get; init; }
    public decimal Amount { get; init; }
    public string Method { get; init; }
    public string? CardNumber { get; init; }
    public string? CardExpiry { get; init; }
    public string? CardCvc { get; init; }

    public ProcessPaymentDto(int invoiceId, decimal amount, string method, string? cardNumber = null, string? cardExpiry = null, string? cardCvc = null)
    {
        InvoiceId = invoiceId;
        Amount = amount;
        Method = method;
        CardNumber = cardNumber;
        CardExpiry = cardExpiry;
        CardCvc = cardCvc;
    }
}

public sealed record RefundPaymentDto
{
    public int PaymentId { get; init; }
    public decimal Amount { get; init; }
    public string Reason { get; init; }
    public string IdempotencyKey { get; init; }

    public RefundPaymentDto(int paymentId, decimal amount, string reason, string idempotencyKey = "")
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
        IdempotencyKey = idempotencyKey;
    }
}

public sealed record InvoiceDto
{
    public int Id { get; init; }
    public string InvoiceNumber { get; init; }
    public string GuestName { get; init; }
    public string RoomNumber { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }

    public InvoiceDto(int id, string invoiceNumber, string guestName, string roomNumber, decimal totalAmount, string status, DateTimeOffset expiresAt)
    {
        Id = id;
        InvoiceNumber = invoiceNumber;
        GuestName = guestName;
        RoomNumber = roomNumber;
        TotalAmount = totalAmount;
        Status = status;
        ExpiresAt = expiresAt;
    }
}

public sealed record PaymentDto
{
    public int Id { get; init; }
    public int InvoiceId { get; init; }
    public string InvoiceNumber { get; init; }
    public string GuestName { get; init; }
    public string RoomNumber { get; init; }
    public decimal Amount { get; init; }
    public string Method { get; init; }
    public string Status { get; init; }

    public PaymentDto(int id, int invoiceId, string invoiceNumber, string guestName, string roomNumber, decimal amount, string method, string status)
    {
        Id = id;
        InvoiceId = invoiceId;
        InvoiceNumber = invoiceNumber;
        GuestName = guestName;
        RoomNumber = roomNumber;
        Amount = amount;
        Method = method;
        Status = status;
    }
}