using HotelOS.PaymentService.DTOs;
using HotelOS.PaymentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentsController(
    IPaymentQueries queries,
    IPaymentCommands commands) : ControllerBase
{
    /// <summary>Retrieves all invoices.</summary>
    /// <remarks>
    /// <para>Example request:</para>
    /// <code>
    /// GET /api/payments/invoices
    /// </code>
    /// <para>Example response:</para>
    /// <code>
    /// [
    ///   {
    ///     "id": 1,
    ///     "invoiceNumber": "INV-2026-001",
    ///     "guestName": "John Doe",
    ///     "totalAmount": 550.00,
    ///     "status": "Pending"
    ///   }
    /// ]
    /// </code>
    /// </remarks>
    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices(CancellationToken cancellationToken)
        => Ok(await queries.GetInvoicesAsync(cancellationToken));

    /// <summary>Retrieves all payment transactions.</summary>
    /// <remarks>
    /// <para>Example request:</para>
    /// <code>
    /// GET /api/payments/transactions
    /// </code>
    /// <para>Example response:</para>
    /// <code>
    /// [
    ///   {
    ///     "id": 1,
    ///     "invoiceId": 1,
    ///     "amount": 250.00,
    ///     "method": "CreditCard",
    ///     "status": "Completed"
    ///   }
    /// ]
    /// </code>
    /// </remarks>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(CancellationToken cancellationToken)
        => Ok(await queries.GetPaymentsAsync(cancellationToken));

    /// <summary>Creates a new invoice.</summary>
    /// <remarks>
    /// <para>Example request:</para>
    /// <code>
    /// POST /api/payments/invoice
    /// {
    ///   "invoiceNumber": "INV-2026-001",
    ///   "guestName": "John Doe",
    ///   "totalAmount": 550.00,
    ///   "roomNightsTotal": 400.00,
    ///   "foodOrdersTotal": 150.00,
    ///   "discountsTotal": 0
    /// }
    /// </code>
    /// <para>Example response:</para>
    /// <code>
    /// {
    ///   "id": 1,
    ///   "invoiceNumber": "INV-2026-001",
    ///   "guestName": "John Doe",
    ///   "totalAmount": 550.00,
    ///   "status": "Pending"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("invoice")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateInvoiceAsync(request, cancellationToken));

    /// <summary>Processes a payment for an invoice.</summary>
    /// <remarks>
    /// <para>Example request:</para>
    /// <code>
    /// POST /api/payments/process
    /// {
    ///   "invoiceId": 1,
    ///   "amount": 250.00,
    ///   "method": "CreditCard"
    /// }
    /// </code>
    /// <para>Example response:</para>
    /// <code>
    /// {
    ///   "id": 1,
    ///   "invoiceId": 1,
    ///   "amount": 250.00,
    ///   "method": "CreditCard",
    ///   "status": "Completed"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto request, CancellationToken cancellationToken)
        => Ok(await commands.ProcessPaymentAsync(request, cancellationToken));

    /// <summary>Refunds a processed payment.</summary>
    /// <remarks>
    /// <para>Example request:</para>
    /// <code>
    /// POST /api/payments/refund
    /// {
    ///   "paymentId": 1,
    ///   "amount": 250.00,
    ///   "reason": "Customer requested cancellation"
    /// }
    /// </code>
    /// <para>Example response:</para>
    /// <code>
    /// {
    ///   "id": 1,
    ///   "invoiceId": 1,
    ///   "amount": 250.00,
    ///   "method": "CreditCard",
    ///   "status": "Refunded"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("refund")]
    public async Task<IActionResult> Refund([FromBody] RefundPaymentDto request, CancellationToken cancellationToken)
        => Ok(await commands.RefundAsync(request, cancellationToken));
}
