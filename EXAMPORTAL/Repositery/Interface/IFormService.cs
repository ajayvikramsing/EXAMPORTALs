using EXAMPORTAL.DTOs;
using EXAMPORTAL.Models;

namespace EXAMPORTAL.Repositery.Interface
{
    public interface IFormService
    {
        Task<Form> CreateAsync(FormCreateDto dto, string creatorId);
        Task<IEnumerable<Form>> ListActiveAsync();
        Task<Form> GetByIdAsync(int id);
        Task<IEnumerable<Form>> AdminListAsync();
        Task UpdateAsync(int id, FormCreateDto dto);
        Task DeleteAsync(int id);
    }
}
