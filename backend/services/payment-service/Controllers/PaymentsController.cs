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
    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices(CancellationToken cancellationToken)
        => Ok(await queries.GetInvoicesAsync(cancellationToken));

    /// <summary>Retrieves all payment transactions.</summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(CancellationToken cancellationToken)
        => Ok(await queries.GetPaymentsAsync(cancellationToken));

    /// <summary>Creates a new invoice.</summary>
    [HttpPost("invoice")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto request, CancellationToken cancellationToken)
        => Ok(await commands.CreateInvoiceAsync(request, cancellationToken));

    /// <summary>Processes a payment for an invoice.</summary>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto request, CancellationToken cancellationToken)
        => Ok(await commands.ProcessPaymentAsync(request, cancellationToken));

    /// <summary>Refunds a processed payment.</summary>
    [HttpPost("refund")]
    public async Task<IActionResult> Refund([FromBody] RefundPaymentDto request, CancellationToken cancellationToken)
        => Ok(await commands.RefundAsync(request, cancellationToken));
}
