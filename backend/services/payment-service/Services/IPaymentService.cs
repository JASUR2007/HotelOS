using HotelOS.PaymentService.DTOs;

namespace HotelOS.PaymentService.Services;

public interface IPaymentService
{
    Task<IReadOnlyList<InvoiceDto>> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(CancellationToken cancellationToken = default);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto request, CancellationToken cancellationToken = default);
    Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto request, CancellationToken cancellationToken = default);
    Task<PaymentDto> RefundAsync(RefundPaymentDto request, CancellationToken cancellationToken = default);
}