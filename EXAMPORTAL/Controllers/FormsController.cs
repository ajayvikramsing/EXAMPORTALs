using EXAMPORTAL.DTOs;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EXAMPORTAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        private readonly IFormService _svc;
        public FormsController(IFormService svc) { _svc = svc; }

        [HttpGet("list")]
        public async Task<IActionResult> List() => Ok(await _svc.ListActiveAsync());

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(FormCreateDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var f = await _svc.CreateAsync(dto, userId);
            return Ok(f);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, FormCreateDto dto)
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _svc.GetByIdAsync(id));
    }
}

