using EXAMPORTAL.DTOs;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EXAMPORTAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _svc;
        public AuthController(IAuthService svc) { _svc = svc; }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var res = await _svc.RegisterAsync(dto);
                return Ok(res);
            }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var res = await _svc.LoginAsync(dto);
                return Ok(res);
            }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }
    }
}
