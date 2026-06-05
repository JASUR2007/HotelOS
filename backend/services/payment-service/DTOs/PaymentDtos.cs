namespace HotelOS.PaymentService.DTOs;

/// <summary>Data transfer object for creating a new invoice.</summary>
public sealed record CreateInvoiceDto
{
    /// <example>INV-2026-001</example>
    public string InvoiceNumber { get; init; }
    /// <example>John Doe</example>
    public string GuestName { get; init; }
    /// <example>101</example>
    public string RoomNumber { get; init; }
    /// <example>550.00</example>
    public decimal TotalAmount { get; init; }
    /// <example>400.00</example>
    public decimal RoomNightsTotal { get; init; }
    /// <example>150.00</example>
    public decimal FoodOrdersTotal { get; init; }
    /// <example>0</example>
    public decimal DiscountsTotal { get; init; }

    public CreateInvoiceDto(string invoiceNumber, string guestName, string roomNumber, decimal totalAmount,
        decimal roomNightsTotal = 0, decimal foodOrdersTotal = 0, decimal discountsTotal = 0)
    {
        InvoiceNumber = invoiceNumber;
        GuestName = guestName;
        RoomNumber = roomNumber;
        TotalAmount = totalAmount;
        RoomNightsTotal = roomNightsTotal;
        FoodOrdersTotal = foodOrdersTotal;
        DiscountsTotal = discountsTotal;
    }
}
/// <summary>Data transfer object for processing a payment.</summary>
public sealed record ProcessPaymentDto
{
    /// <example>1</example>
    public int InvoiceId { get; init; }
    /// <example>250.00</example>
    public decimal Amount { get; init; }
    /// <example>CreditCard</example>
    public string Method { get; init; }

    public ProcessPaymentDto(int invoiceId, decimal amount, string method)
    {
        InvoiceId = invoiceId;
        Amount = amount;
        Method = method;
    }
}
/// <summary>Data transfer object for refunding a payment.</summary>
public sealed record RefundPaymentDto
{
    /// <example>1</example>
    public int PaymentId { get; init; }
    /// <example>250.00</example>
    public decimal Amount { get; init; }
    /// <example>Customer requested cancellation</example>
    public string Reason { get; init; }

    public RefundPaymentDto(int paymentId, decimal amount, string reason)
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
    }
}
/// <summary>Data transfer object representing an invoice.</summary>
public sealed record InvoiceDto
{
    /// <example>1</example>
    public int Id { get; init; }
    /// <example>INV-2026-001</example>
    public string InvoiceNumber { get; init; }
    /// <example>John Doe</example>
    public string GuestName { get; init; }
    /// <example>101</example>
    public string RoomNumber { get; init; }
    /// <example>550.00</example>
    public decimal TotalAmount { get; init; }
    /// <example>Pending</example>
    public string Status { get; init; }

    public InvoiceDto(int id, string invoiceNumber, string guestName, string roomNumber, decimal totalAmount, string status)
    {
        Id = id;
        InvoiceNumber = invoiceNumber;
        GuestName = guestName;
        RoomNumber = roomNumber;
        TotalAmount = totalAmount;
        Status = status;
    }
}
/// <summary>Data transfer object representing a payment transaction.</summary>
public sealed record PaymentDto
{
    /// <example>1</example>
    public int Id { get; init; }
    /// <example>1</example>
    public int InvoiceId { get; init; }
    /// <example>INV-10021</example>
    public string InvoiceNumber { get; init; }
    /// <example>John Doe</example>
    public string GuestName { get; init; }
    /// <example>101</example>
    public string RoomNumber { get; init; }
    /// <example>250.00</example>
    public decimal Amount { get; init; }
    /// <example>CreditCard</example>
    public string Method { get; init; }
    /// <example>Completed</example>
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