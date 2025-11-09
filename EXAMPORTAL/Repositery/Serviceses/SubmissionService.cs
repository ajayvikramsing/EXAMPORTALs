using EXAMPORTAL.Data;
using EXAMPORTAL.DTOs;
using EXAMPORTAL.Models;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.EntityFrameworkCore;

namespace EXAMPORTAL.Repositery.Serviceses
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ApplicationDbContext _db;
        public SubmissionService(ApplicationDbContext db) { _db = db; }

        public async Task<Submission> CreateAsync(SubmissionCreateDto dto, string userId)
        {
            var form = await _db.Forms.FindAsync(dto.FormId);
            if (form == null || !form.IsActive) throw new ApplicationException("Form unavailable");

            var sub = new Submission
            {
                FormId = dto.FormId,
                AnswersJson = dto.AnswersJson,
                UserId = userId,
                Status = "PendingPayment"
            };
            _db.Submissions.Add(sub);
            await _db.SaveChangesAsync();
            return sub;
        }

        public async Task<Submission> GetByIdAsync(int id) => await _db.Submissions.Include(s => s.Payment).Include(s => s.Form).FirstOrDefaultAsync(s => s.Id == id);

        public async Task<IEnumerable<Submission>> GetByUserAsync(string userId) => await _db.Submissions.Where(s => s.UserId == userId).Include(s => s.Form).ToListAsync();

        public async Task<IEnumerable<Submission>> AdminListAsync() => await _db.Submissions.Include(s => s.Form).Include(s => s.Payment).ToListAsync();

        public async Task UpdateStatusAsync(int submissionId, string status)
        {
            var s = await _db.Submissions.FindAsync(submissionId);
            if (s == null) throw new KeyNotFoundException("Submission not found");
            s.Status = status;
            await _db.SaveChangesAsync();
        }
    }
   
}
