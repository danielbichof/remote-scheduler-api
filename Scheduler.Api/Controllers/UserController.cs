using Microsoft.AspNetCore.Mvc;
using Scheduler.Application.DTOs;
using Scheduler.Application.Services;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        try
        {
            await _userService.RegisterUserAsync(registerDto);
            return Ok(new { message = "Usuário registrado com sucesso. Um e-mail de confirmação foi enviado." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userService.AuthenticateAsync(request.Email, request.Password);
        if (result == null)
        {
            return Unauthorized(new { message = "Credenciais inválidas" });
        }

        return Ok(new { userId = result?.userId, username = result?.username, email = result?.email });
    }
}