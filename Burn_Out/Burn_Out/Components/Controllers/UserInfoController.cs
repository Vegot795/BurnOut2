using Burn_Out.Client;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/userinfo")]
public class UserInfoController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserInfoController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<UserInfo> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            throw new Exception("User not found.");

        return new UserInfo
        {
            UserId = user.Id,       // ✅ required
            Email = user.Email,     // ✅ required
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = string.IsNullOrEmpty(user.PhoneNumber) ? null : int.Parse(user.PhoneNumber),
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt,
        };
    }
}
