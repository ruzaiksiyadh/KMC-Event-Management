using Microsoft.AspNetCore.Identity;

namespace KMC.Api.Models;

// An organizer (or council admin) who can log in and create events.
// Public site visitors never need an account - only organizers do.
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}
