namespace EXAMPORTAL.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }

        // answers stored as JSON string
        public string AnswersJson { get; set; }

        // "PendingPayment", "Paid", "Cancelled"
        public string Status { get; set; } = "PendingPayment";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Payment Payment { get; set; }
    }
}
