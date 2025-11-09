using EXAMPORTAL.DTOs;
using EXAMPORTAL.Models;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.AspNetCore.Identity;

namespace EXAMPORTAL.Repositery.Serviceses
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly JwtTokenGenerator _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, JwtTokenGenerator jwt)
        {
            _userManager = userManager;
            _config = config;
            _jwt = jwt;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new ApplicationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            // assign 'User' role by default
            await _userManager.AddToRoleAsync(user, "User");

            var token = await _jwt.GenerateTokenAsync(user);
            return new AuthResponseDto { Token = token, Role = "User", Email = user.Email };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) throw new ApplicationException("Invalid credentials");
            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) throw new ApplicationException("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _jwt.GenerateTokenAsync(user);
            return new AuthResponseDto { Token = token, Role = roles.FirstOrDefault() ?? "User", Email = user.Email };
        }
    }
}
