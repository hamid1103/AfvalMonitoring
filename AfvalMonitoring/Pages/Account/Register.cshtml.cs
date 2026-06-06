using AfvalMonitoring.Data;
using AfvalMonitoring.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AfvalMonitoring.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly DataDbContext _db;
    private readonly IPasswordHasher<AppUser> _hasher;

    public RegisterModel(DataDbContext db, IPasswordHasher<AppUser> hasher)
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
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.Password != Input.ConfirmPassword)
        {
            ErrorMessage = "Wachtwoorden komen niet overeen.";
            return Page();
        }

        if (_db.AppUsers.Any(u => u.Username == Input.Username))
        {
            ErrorMessage = "Gebruikersnaam al in gebruik.";
            return Page();
        }

        var user = new AppUser { Username = Input.Username };
        user.PasswordHash = _hasher.HashPassword(user, Input.Password);

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        return Redirect("/Account/Login");
    }
}
