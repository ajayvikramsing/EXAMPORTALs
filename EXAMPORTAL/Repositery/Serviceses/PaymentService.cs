using EXAMPORTAL.Data;
using EXAMPORTAL.DTOs;
using EXAMPORTAL.Models;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Stripe;

namespace EXAMPORTAL.Repositery.Serviceses
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _cfg;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IConfiguration cfg, ApplicationDbContext db, ILogger<PaymentService> logger)
        {
            _cfg = cfg;
            _db = db;
            _logger = logger;
            StripeConfiguration.ApiKey = _cfg["Stripe:SecretKey"];
        }

        public async Task<object> CreatePaymentAsync(PaymentCreateDto dto)
        {
            var submission = await _db.Submissions.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == dto.SubmissionId);
            if (submission == null) throw new ApplicationException("Submission not found");

            // Create Stripe PaymentIntent
            var options = new PaymentIntentCreateOptions
            {
                Amount = dto.Amount,
                Currency = dto.Currency,
                Metadata = new Dictionary<string, string> { { "submissionId", dto.SubmissionId.ToString() } },
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true }
            };
            var service = new PaymentIntentService();
            var pi = await service.CreateAsync(options);

            // create a Payment record with pending state
            var payment = new Payment
            {
                SubmissionId = dto.SubmissionId,
                Provider = "Stripe",
                OrderId = pi.Id,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = "pending"
            };
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return new { clientSecret = pi.ClientSecret, paymentIntentId = pi.Id };
        }

        public async Task HandleStripeWebhookAsync(string json, string sigHeader)
        {
            var endpointSecret = _cfg["Stripe:WebhookSecret"];
            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, sigHeader, endpointSecret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed.");
                throw;
            }

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                var pi = stripeEvent.Data.Object as PaymentIntent;
                var submissionIdStr = pi.Metadata.ContainsKey("submissionId") ? pi.Metadata["submissionId"] : null;
                if (int.TryParse(submissionIdStr, out var submissionId))
                {
                    var payment = await _db.Payments.FirstOrDefaultAsync(p => p.OrderId == pi.Id);
                    if (payment != null)
                    {
                        payment.PaymentId = pi.Id;
                        payment.Status = "succeeded";
                        payment.Signature = stripeEvent.Id;
                        await _db.SaveChangesAsync();

                        // update submission
                        var submission = await _db.Submissions.FindAsync(submissionId);
                        if (submission != null)
                        {
                            submission.Status = "Paid";
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
            // handle other events as needed
        }

        public async Task<Payment> GetByIdAsync(int id) => await _db.Payments.Include(p => p.Submission).ThenInclude(s => s.User).FirstOrDefaultAsync(p => p.Id == id);

        public async Task GenerateReceiptPdfAsync(int paymentId, Stream output)
        {
            var payment = await GetByIdAsync(paymentId);
            if (payment == null) throw new KeyNotFoundException("Payment not found");

            // Create a simple receipt using QuestPDF
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Receipt - Order {payment.OrderId}").Bold().FontSize(18);
                        col.Item().Text($"Date: {payment.CreatedAt:yyyy-MM-dd HH:mm} UTC");
                        col.Item().Text($"Paid By: {payment.Submission.User?.FullName ?? payment.Submission.User?.Email}");
                        col.Item().Text($"Email: {payment.Submission.User?.Email}");
                        col.Item().Text($"Submission ID: {payment.SubmissionId}");
                        col.Item().Text($"Amount: {(payment.Amount / 100.0):0.00} {payment.Currency}");
                        col.Item().Text($"Provider: {payment.Provider}");
                        col.Item().PaddingTop(10).Text("Thank you for your payment.").Italic();
                    });
                });
            });

            doc.GeneratePdf(output);
            output.Seek(0, SeekOrigin.Begin);
        }
    }
   
}
