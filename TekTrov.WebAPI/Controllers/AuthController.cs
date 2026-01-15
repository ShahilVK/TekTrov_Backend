
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
    ApiResponse<bool>.SuccessResponse(
        true,
        "User registered successfully",
        201
    ));

    }




    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var result = await _userService.LoginAsync(dto);

        HttpContext.Session.SetString(
       "RefreshToken",
       result.RefreshToken
   );

        return Ok(ApiResponse<object>.SuccessResponse(
       new
       {
           accessToken = result.AccessToken
       },
       "Login successful"
   ));
    }





    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();

        return Ok(
            ApiResponse<bool>.SuccessResponse(
                true,
                "Logout successful"));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken =
            HttpContext.Session.GetString("RefreshToken");

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(ApiResponse<object>.FailureResponse(
                "Session expired", 401));

        var result =
            await _userService.RefreshTokenAsync(refreshToken);

        HttpContext.Session.SetString(
            "RefreshToken",
            result.RefreshToken
        );

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                accessToken = result.AccessToken
            },
            "Token refreshed"
        ));
    }


  
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPasswordWithOtp(
     [FromBody] ResetPasswordWithOtpDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid data", 400));

        await _userService.ResetPasswordWithOtpAsync(dto);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Password reset successful"
        ));
    }


    [HttpPost("password/send-otp")]
    public async Task<IActionResult> SendPasswordOtp(
     [FromBody] SendPasswordOtpDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Invalid email", 400));

        await _userService.SendPasswordOtpAsync(dto);

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "If email exists, OTP has been sent"
        ));
    }


}
