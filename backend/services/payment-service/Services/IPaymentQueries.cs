using HotelOS.PaymentService.DTOs;

namespace HotelOS.PaymentService.Services;

public interface IPaymentQueries
{
    Task<IReadOnlyList<InvoiceDto>> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(CancellationToken cancellationToken = default);
}
