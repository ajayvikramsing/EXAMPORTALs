namespace EXAMPORTAL.DTOs
{
    public class SubmissionCreateDto
    {
        public int FormId { get; set; }
        public string AnswersJson { get; set; }
    }

    public class PaymentCreateDto
    {
        public int SubmissionId { get; set; }
        public long Amount { get; set; } // smallest currency unit
        public string Currency { get; set; } = "INR";
        public string Provider { get; set; } = "Stripe";
    }
}
