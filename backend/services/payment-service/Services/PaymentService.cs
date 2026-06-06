using HotelOS.PaymentService.DTOs;
using HotelOS.PaymentService.Events;
using HotelOS.PaymentService.Models;
using HotelOS.PaymentService.Repositories;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;

namespace HotelOS.PaymentService.Services;

public sealed class PaymentService(
    IPaymentRepository repository,
    IEventPublisher eventPublisher,
    BillingCalculationService billingService,
    ILogger<PaymentService> logger) : IPaymentService, IPaymentQueries, IPaymentCommands
{
    public async Task<IReadOnlyList<InvoiceDto>> GetInvoicesAsync(CancellationToken cancellationToken = default)
        => (await repository.GetInvoicesAsync(cancellationToken))
            .Select(invoice => new InvoiceDto(invoice.Id, invoice.InvoiceNumber, invoice.GuestName, invoice.RoomNumber, invoice.TotalAmount, invoice.Status))
            .ToList();

    public async Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetPaymentsWithInvoicesAsync(cancellationToken))
            .Select(tuple => new PaymentDto(
                tuple.Payment.Id,
                tuple.Payment.InvoiceId,
                tuple.Invoice.InvoiceNumber,
                tuple.Invoice.GuestName,
                tuple.Invoice.RoomNumber,
                tuple.Payment.Amount,
                tuple.Payment.Method,
                tuple.Payment.Status))
            .ToList();

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto request, CancellationToken cancellationToken = default)
    {
        var billingResult = billingService.Calculate(new BillingInput(
            request.RoomNightsTotal,
            request.FoodOrdersTotal,
            request.MinibarTotal,
            request.DamagesTotal,
            request.DiscountsTotal));

        var invoice = await repository.CreateInvoiceAsync(new Invoice
        {
            InvoiceNumber = request.InvoiceNumber,
            GuestName = request.GuestName,
            RoomNumber = request.RoomNumber,
            TotalAmount = billingResult.NetTotal,
            Status = "Open"
        }, cancellationToken);

        try
        {
            eventPublisher.Publish("invoice.generated", new
            {
                invoice.Id,
                invoice.InvoiceNumber,
                invoice.GuestName,
                billingResult.GrossTotal,
                billingResult.NetTotal,
                OccurredAt = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish invoice.generated event for invoice {InvoiceId}", invoice.Id);
        }

        _ = new InvoiceGeneratedEvent(invoice.Id, invoice.InvoiceNumber, DateTimeOffset.UtcNow);
        return new InvoiceDto(invoice.Id, invoice.InvoiceNumber, invoice.GuestName, invoice.RoomNumber, invoice.TotalAmount, invoice.Status);
    }

    public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto request, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            throw new ArgumentException($"Invoice with ID {request.InvoiceId} does not exist.");
        }

        var payment = await repository.CreatePaymentAsync(new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            Method = request.Method,
            Status = "Completed",
            ProcessedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        try
        {
            eventPublisher.Publish(RabbitMqRoutingKeys.PaymentCompleted, new
            {
                payment.Id,
                payment.InvoiceId,
                payment.Amount,
                payment.Method,
                OccurredAt = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish PaymentCompleted event for payment {PaymentId}", payment.Id);
        }

        return new PaymentDto(payment.Id, payment.InvoiceId, string.Empty, string.Empty, string.Empty, payment.Amount, payment.Method, payment.Status);
    }

    public async Task<PaymentDto> RefundAsync(RefundPaymentDto request, CancellationToken cancellationToken = default)
    {
        var payment = await repository.CreatePaymentAsync(new Payment
        {
            InvoiceId = request.PaymentId,
            Amount = -request.Amount,
            Method = "Refund",
            Status = "Refunded",
            ProcessedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        try
        {
            eventPublisher.Publish(RabbitMqRoutingKeys.PaymentRefunded, new
            {
                payment.Id,
                request.PaymentId,
                request.Amount,
                request.Reason,
                OccurredAt = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish PaymentRefunded event for payment {PaymentId}", payment.Id);
        }

        _ = new RefundProcessedEvent(payment.Id, request.Reason, DateTimeOffset.UtcNow);
        return new PaymentDto(payment.Id, payment.InvoiceId, string.Empty, string.Empty, string.Empty, payment.Amount, payment.Method, payment.Status);
    }
}
