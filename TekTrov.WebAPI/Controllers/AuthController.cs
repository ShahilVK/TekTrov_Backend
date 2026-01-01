
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TekTrov.Application.DTOs.Auth;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    //[HttpPost("register")]
    //public async Task<IActionResult> Register(RegisterDTO dto)
    //{
    //    await _userService.RegisterAsync(dto);
    //    return Ok("User registered successfully");
    //}

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _userService.RegisterAsync(dto);
        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var result = await _userService.LoginAsync(dto);
        return Ok(result);
    }



    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value
        );

        await _userService.LogoutAsync(userId);
        return Ok("Logout successful");
    }
}
