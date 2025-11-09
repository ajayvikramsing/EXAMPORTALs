namespace EXAMPORTAL.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public Submission Submission { get; set; }

        public string Provider { get; set; } // "Stripe" or "Razorpay"
        public string OrderId { get; set; }  // provider order id or payment intent id
        public string PaymentId { get; set; } // provider payment id
        public long Amount { get; set; } // in smallest currency unit (e.g., cents)
        public string Currency { get; set; } = "INR";
        public string Status { get; set; } // "succeeded", "failed", "pending"
        public string Signature { get; set; } // webhook signature (stored for audit)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
