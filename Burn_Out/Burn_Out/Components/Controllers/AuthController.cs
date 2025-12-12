using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Clear any existing external cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var result = await _signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in.", request.Email);

                // Get user info to return
                var user = await _userManager.FindByEmailAsync(request.Email);

                return Ok(new LoginResponse
                {
                    Succeeded = true,
                    UserId = user?.Id,
                    Email = user?.Email,
                    UserName = user?.UserName
                });
            }
            else if (result.RequiresTwoFactor)
            {
                return Ok(new LoginResponse
                {
                    RequiresTwoFactor = true
                });
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User account {Email} locked out.", request.Email);
                return Unauthorized(new { error = "Account locked out" });
            }
            else
            {
                _logger.LogWarning("Invalid login attempt for {Email}", request.Email);
                return Unauthorized(new { error = "Invalid login attempt" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return StatusCode(500, new { error = "An error occurred during login" });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return Ok(new { succeeded = true });
        NavigationManager.NavigateTo("/", forceLoad: true);

    }

    [HttpGet("check-auth")]
    public async Task<IActionResult> CheckAuth()
    {
        var user = await _signInManager.UserManager.GetUserAsync(User);
        if (user != null)
        {
            return Ok(new
            {
                isAuthenticated = true,
                email = user.Email,
                userName = user.UserName
            });
        }

        return Ok(new { isAuthenticated = false });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}

public class LoginResponse
{
    public bool Succeeded { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
}