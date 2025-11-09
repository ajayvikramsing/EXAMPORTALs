using EXAMPORTAL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EXAMPORTAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) { _db = db; }

        [HttpGet("stats")]
        public async Task<IActionResult> Stats()
        {
            var forms = await _db.Forms.CountAsync();
            var submissions = await _db.Submissions.CountAsync();
            var payments = await _db.Payments.CountAsync();
            return Ok(new { forms, submissions, payments });
        }
    }
}
