using EXAMPORTAL.DTOs;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EXAMPORTAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _svc;
        public SubmissionsController(ISubmissionService svc) { _svc = svc; }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(SubmissionCreateDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var s = await _svc.CreateAsync(dto, userId);
            return Ok(s);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> Mine()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _svc.GetByUserAsync(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/list")]
        public async Task<IActionResult> AdminList() => Ok(await _svc.AdminListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _svc.GetByIdAsync(id));
    }
}

