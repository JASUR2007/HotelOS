using HotelOS.PaymentService.DTOs;

namespace HotelOS.PaymentService.Services;

public interface IPaymentCommands
{
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto request, CancellationToken cancellationToken = default);
    Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto request, CancellationToken cancellationToken = default);
    Task<PaymentDto> RefundAsync(RefundPaymentDto request, CancellationToken cancellationToken = default);
}
