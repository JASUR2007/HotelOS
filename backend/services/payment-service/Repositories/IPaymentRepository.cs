using HotelOS.PaymentService.Models;

namespace HotelOS.PaymentService.Repositories;

public interface IPaymentRepository
{
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Payment> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(Payment Payment, Invoice Invoice)>> GetPaymentsWithInvoicesAsync(CancellationToken cancellationToken = default);
}