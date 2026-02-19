using Microsoft.AspNetCore.Identity;

namespace SkiSettlement.Data.Models;

/// <summary>Użytkownik systemu (logowanie e-mail + hasło).</summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
