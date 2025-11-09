using EXAMPORTAL.DTOs;
using EXAMPORTAL.Models;

namespace EXAMPORTAL.Repositery.Interface
{
    public interface IPaymentService
    {
        // For Stripe: create payment intent and return client secret / id
        Task<object> CreatePaymentAsync(PaymentCreateDto dto);
        Task HandleStripeWebhookAsync(string json, string sigHeader);
        Task<Payment> GetByIdAsync(int id);
        Task GenerateReceiptPdfAsync(int paymentId, Stream output);
    }
}
