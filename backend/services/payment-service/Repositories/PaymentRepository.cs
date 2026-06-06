using HotelOS.PaymentService.Data;
using HotelOS.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.PaymentService.Repositories;

public sealed class PaymentRepository(PaymentDbContext context) : IPaymentRepository
{
    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync(cancellationToken);
        return invoice;
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        context.Payments.Add(payment);
        await context.SaveChangesAsync(cancellationToken);
        return payment;
    }

    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken cancellationToken = default)
        => await context.Invoices.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default)
        => await context.Payments.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, CancellationToken cancellationToken = default)
        => await context.Invoices.AsNoTracking().FirstOrDefaultAsync(invoice => invoice.Id == invoiceId, cancellationToken);

    public async Task<IReadOnlyList<(Payment Payment, Invoice Invoice)>> GetPaymentsWithInvoicesAsync(CancellationToken cancellationToken = default)
        => await (from payment in context.Payments.AsNoTracking()
                  join invoice in context.Invoices.AsNoTracking() on payment.InvoiceId equals invoice.Id
                  select new ValueTuple<Payment, Invoice>(payment, invoice))
                  .ToListAsync(cancellationToken);

    public async Task<IdempotentRefund?> GetIdempotentRefundAsync(string idempotencyKey, CancellationToken cancellationToken = default)
        => await context.IdempotentRefunds.FindAsync([idempotencyKey], cancellationToken);

    public async Task SaveIdempotentRefundAsync(IdempotentRefund record, CancellationToken cancellationToken = default)
    {
        context.IdempotentRefunds.Add(record);
        await context.SaveChangesAsync(cancellationToken);
    }
}