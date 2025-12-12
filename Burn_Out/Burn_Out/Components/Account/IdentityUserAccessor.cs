using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace Burn_Out.Components.Account
{
    internal sealed class IdentityUserAccessor
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IdentityRedirectManager _redirectManager;

        public IdentityUserAccessor(
            UserManager<ApplicationUser> userManager,
            AuthenticationStateProvider authenticationStateProvider,
            IdentityRedirectManager redirectManager)
        {
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
            _redirectManager = redirectManager;
        }

        public async Task<ApplicationUser> GetRequiredUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var principal = authState.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                _redirectManager.RedirectToWithStatus(
                    "Account/InvalidUser",
                    "Error: User is not authenticated.",
                    null);
                throw new InvalidOperationException("User is not authenticated");
            }

            var user = await _userManager.GetUserAsync(principal);

            if (user is null)
            {
                _redirectManager.RedirectToWithStatus(
                    "Account/InvalidUser",
                    $"Error: Unable to load user with ID '{_userManager.GetUserId(principal)}'.",
                    null);
                throw new InvalidOperationException("User not found");
            }

            return user;
        }
    }
}
