using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXAMPORTAL.Models
{
    public class Form
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        [Required]
        public string FieldsJson { get; set; } = default!;

        public bool IsActive { get; set; }

        // ✅ Must be nullable for DeleteBehavior.SetNull
        public string? CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
