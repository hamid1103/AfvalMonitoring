using System.Security.Claims;
using AfvalMonitoring.Data;
using AfvalMonitoring.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AfvalMonitoring.Pages.Account;

public class LoginModel : PageModel
{
    private readonly DataDbContext _db;
    private readonly IPasswordHasher<AppUser> _hasher;

    public LoginModel(DataDbContext db, IPasswordHasher<AppUser> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = _db.AppUsers.FirstOrDefault(u => u.Username == Input.Username);
        if (user == null)
        {
            ErrorMessage = "Gebruikersnaam of wachtwoord onjuist.";
            return Page();
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Gebruikersnaam of wachtwoord onjuist.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties { IsPersistent = false };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);

        return Redirect("/voorspelling");
    }
}
