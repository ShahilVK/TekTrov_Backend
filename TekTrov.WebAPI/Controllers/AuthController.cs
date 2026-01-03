
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TekTrov.Application.Common;
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


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<object>.FailureResponse(
                    "Validation failed", 400));
        }

        await _userService.RegisterAsync(dto);

        return StatusCode(201,
    ApiResponse<object>.SuccessResponse(
        null,
        "User registered successfully",
        201
    ));

    }




    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var result = await _userService.LoginAsync(dto);

        return Ok(
            ApiResponse<object>.SuccessResponse(
                result,
                "Login successful"));
    }





    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(
     User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value
 );


        await _userService.LogoutAsync(userId);

        return Ok(
            ApiResponse<object>.SuccessResponse(
                null,
                "Logout successful"));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest(
                ApiResponse<object>.FailureResponse(
                    "Refresh token is required", 400
                )
            );

        var result = await _userService.RefreshTokenAsync(dto.RefreshToken);

        return Ok(
            ApiResponse<AuthResponseDTO>.SuccessResponse(
                result,
                "Token refreshed successfully"
            )
        );
    }

}
