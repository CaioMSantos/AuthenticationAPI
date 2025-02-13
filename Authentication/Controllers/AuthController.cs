using Microsoft.AspNetCore.Mvc;
using Authentication.Data;
using Authentication.Models;
using Authentication.Services;
using Microsoft.EntityFrameworkCore;


namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _authService.UserExists(user.Username))
                return BadRequest("Usuário já existe!");

            await _authService.RegisterUser(user);
            return Ok("Usuário registrado com sucesso!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var dbUser = await _authService.GetUserByUsername(user.Username);
            if (dbUser == null || !_authService.VerifyPassword(user.PasswordHash, dbUser.PasswordHash))
                return Unauthorized("Usuário ou senha inválidos!");

            var token = _authService.GenerateToken(dbUser);
            return Ok(new { Token = token });
        }
    }
}
