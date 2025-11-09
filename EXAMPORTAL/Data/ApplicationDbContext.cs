using EXAMPORTAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EXAMPORTAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<Form> Forms { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Safe relationship: allows user deletion without breaking FK
            builder.Entity<Form>()
                .HasOne(f => f.CreatedBy)
                .WithMany()
                .HasForeignKey(f => f.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Submission>()
                .HasOne(s => s.Form)
                .WithMany()
                .HasForeignKey(s => s.FormId);

            builder.Entity<Submission>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);

            builder.Entity<Payment>()
                .HasOne(p => p.Submission)
                .WithOne(s => s.Payment)
                .HasForeignKey<Payment>(p => p.SubmissionId);
        }
    }
}
