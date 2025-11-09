using EXAMPORTAL.Data;
using EXAMPORTAL.DTOs;
using EXAMPORTAL.Models;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.EntityFrameworkCore;

namespace EXAMPORTAL.Repositery.Serviceses
{
    public class FormService : IFormService
    {
        private readonly ApplicationDbContext _db;
        public FormService(ApplicationDbContext db) { _db = db; }

        public async Task<Form> CreateAsync(FormCreateDto dto, string creatorId)
        {
            var f = new Form
            {
                Title = dto.Title,
                Description = dto.Description,
                FieldsJson = dto.FieldsJson,
                IsActive = dto.IsActive,
                CreatedById = creatorId
            };
            _db.Forms.Add(f);
            await _db.SaveChangesAsync();
            return f;
        }

        public async Task DeleteAsync(int id)
        {
            var f = await _db.Forms.FindAsync(id);
            if (f == null) throw new KeyNotFoundException("Form not found");
            _db.Forms.Remove(f);
            await _db.SaveChangesAsync();
        }

        public async Task<Form> GetByIdAsync(int id) => await _db.Forms.FirstOrDefaultAsync(f => f.Id == id);

        public async Task<IEnumerable<Form>> ListActiveAsync() => await _db.Forms.Where(f => f.IsActive).ToListAsync();

        public async Task<IEnumerable<Form>> AdminListAsync() => await _db.Forms.ToListAsync();

        public async Task UpdateAsync(int id, FormCreateDto dto)
        {
            var f = await _db.Forms.FindAsync(id);
            if (f == null) throw new KeyNotFoundException("Form not found");
            f.Title = dto.Title;
            f.Description = dto.Description;
            f.FieldsJson = dto.FieldsJson;
            f.IsActive = dto.IsActive;
            await _db.SaveChangesAsync();
        }
    }
   
}
