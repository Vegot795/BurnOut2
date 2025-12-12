using Burn_Out.Components.Account;
using Burn_Out.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Infrastructure.Data;
using Core.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Configure EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure authentication cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.SlidingExpiration = true;
});

// Add authentication/authorization state for Blazor
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

// Add authentication for WebAssembly
builder.Services.AddAuthorizationCore();

// Custom repositories
builder.Services.AddScoped<IHallRepository, HallRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Fix: Configure HttpClient without NavigationManager
builder.Services.AddHttpClient("ServerAPI", (sp, client) =>
{
    // Get the current request's base address
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;

    if (request != null)
    {
        client.BaseAddress = new Uri($"{request.Scheme}://{request.Host}/");
    }
    else
    {
        // Fallback for WebAssembly or when not in HTTP context
        client.BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? "https://localhost:5001/");
    }
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseCookies = true,
    AllowAutoRedirect = false
});

// Add a default HttpClient for general use
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? "https://localhost:5001/")
    });

var app = builder.Build();

// Configure appsettings.json to include BaseUrl
// Add to appsettings.json:
// {
//   "BaseUrl": "https://localhost:5001"
// }

// Seed roles/admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await IdentitySeeder.SeedRolesAndAdminAsync(roleManager, userManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Map Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Burn_Out.Client._Imports).Assembly);

// Enhanced login endpoint
app.MapPost("/api/auth/login", async (
    LoginModel model,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    HttpContext context) =>
{
    try
    {
        // Clear any existing external cookie
        await context.SignOutAsync(IdentityConstants.ExternalScheme);

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Results.Json(new { success = false, message = "Invalid login attempt" }, statusCode: 400);

        // Sign in the user
        var result = await signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return Results.Json(new { success = true });
        }
        else if (result.IsLockedOut)
        {
            return Results.Json(new { success = false, message = "Account locked out" }, statusCode: 423);
        }
        else
        {
            return Results.Json(new { success = false, message = "Invalid login attempt" }, statusCode: 400);
        }
    }
    catch (Exception ex)
    {
        return Results.Json(new { success = false, message = "An error occurred" }, statusCode: 500);
    }
});

// Add logout endpoint for WebAssembly
app.MapPost("/api/auth/logout", async (
    SignInManager<ApplicationUser> signInManager,
    HttpContext context) =>
{
    await signInManager.SignOutAsync();
    return Results.Json(new { success = true });
});

// Add authentication check endpoint
app.MapGet("/api/auth/check", async (
    HttpContext context,
    UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.GetUserAsync(context.User);
    if (user != null)
    {
        return Results.Json(new
        {
            isAuthenticated = true,
            email = user.Email,
            userName = user.UserName,
            userId = user.Id
        });
    }

    return Results.Json(new { isAuthenticated = false });
});

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

// Login model for API
public class LoginModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}