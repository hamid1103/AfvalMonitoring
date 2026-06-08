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
        if (string.IsNullOrWhiteSpace(Input.Username) || Input.Username.Length < 6)
        {
            ErrorMessage = "Gebruikersnaam moet minimaal 6 tekens zijn.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Input.Password))
        {
            ErrorMessage = "Voer een wachtwoord in.";
            return Page();
        }

        if (Input.Password != Input.ConfirmPassword)
        {
            ErrorMessage = "Wachtwoorden komen niet overeen.";
            return Page();
        }

        try
        {
            if (_db.AppUsers.Any(u => u.Username == Input.Username))
            {
                ErrorMessage = "Gebruikersnaam al in gebruik.";
                return Page();
            }

            var user = new AppUser { Username = Input.Username };
            user.PasswordHash = _hasher.HashPassword(user, Input.Password);

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register database error: {ex.Message}");
            ErrorMessage = "Kan geen verbinding maken met de database. Probeer het later opnieuw.";
            return Page();
        }

        return Redirect("/Account/Login");
    }
}
