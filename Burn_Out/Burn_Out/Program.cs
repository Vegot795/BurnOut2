using Burn_Out.Components;
using Burn_Out.Components.Account;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Server;


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
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Burn_Out")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireRole("Admin"));

    options.AddPolicy("Staff",
        policy => policy.RequireRole("Admin", "Employee"));
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure authentication cookies

// Add authentication/authorization state for Blazor
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();


// Add authentication for WebAssembly

// Custom repositories
builder.Services.AddScoped<IHallRepository, HallRepository>();
builder.Services.AddScoped<Application.Services.HallReservationService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Fix: Configure HttpClient without NavigationManager
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("LocalApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:5230");
});



// Add a default HttpClient for general use
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? "https://localhost:5230/")
    });

builder.Services.Configure<CircuitOptions>(options => { options.DetailedErrors = true; });




var app = builder.Build();

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


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedRolesAsync(services);
}

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Burn_Out.Client._Imports).Assembly);


app.MapGet("/auth/login", async (
    string email,
    string password,
    SignInManager<ApplicationUser> signInManager) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/login?error=1");
    }

    var result = await signInManager.PasswordSignInAsync(
        email,
        password,
        isPersistent: true,
        lockoutOnFailure: false);

    return result.Succeeded
        ? Results.Redirect("/userProfile")
        : Results.Redirect("/login?error=1");
});

app.MapGet("/auth/signin", async (
    string email,
    string password,
    SignInManager<ApplicationUser> signInManager) =>
{
    var result = await signInManager.PasswordSignInAsync(
        email,
        password,
        isPersistent: false,
        lockoutOnFailure: false);

    return result.Succeeded
        ? Results.Redirect("/")
        : Results.Redirect("/login?error=1");
});


app.MapPost("/api/auth/logout", async (
    SignInManager<ApplicationUser> signInManager,
    HttpContext context) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
    });




// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

// Login model for API
