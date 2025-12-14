using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
            return Unauthorized();

        return Ok();
    }
}

public record LoginRequest(string Email, string Password, bool RememberMe);
