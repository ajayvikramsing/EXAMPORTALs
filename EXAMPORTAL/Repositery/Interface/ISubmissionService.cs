using EXAMPORTAL.DTOs;
using EXAMPORTAL.Models;

namespace EXAMPORTAL.Repositery.Interface
{
    public interface ISubmissionService
    {
        Task<Submission> CreateAsync(SubmissionCreateDto dto, string userId);
        Task<Submission> GetByIdAsync(int id);
        Task<IEnumerable<Submission>> GetByUserAsync(string userId);
        Task<IEnumerable<Submission>> AdminListAsync();
        Task UpdateStatusAsync(int submissionId, string status);
    }
}
