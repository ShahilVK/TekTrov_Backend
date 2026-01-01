using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")] // 🔒 ADMIN ONLY
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // ✅ GET /api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound("User not found");

        return Ok(user);
    }
}
