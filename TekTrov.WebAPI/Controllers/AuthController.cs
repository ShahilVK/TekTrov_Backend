
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
          
            return BadRequest(ApiResponse<object>.FailureResponse(
    ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage)
        .FirstOrDefault() ?? "Validation failed",
    400));

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
        try
        {
            var result = await _userService.LoginAsync(dto);


            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(7)
            });


            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    accessToken = result.AccessToken
                },
                "Login successful"
            ));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<object>.FailureResponse(ex.Message, 401)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<object>.FailureResponse(ex.Message, 400)
            );
        }
    }






    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
      
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            Path = "/"
        });

        return Ok(ApiResponse<bool>.SuccessResponse(
            true,
            "Logout successful"
        ));

    }
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {

        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized(ApiResponse<object>.FailureResponse(
                "Refresh token missing", 401));
        }


        var result =
            await _userService.RefreshTokenAsync(refreshToken);

        Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });

        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(7)
        });

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
