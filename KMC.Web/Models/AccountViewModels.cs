namespace KMC.Web.Models;

public class LoginViewModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Error { get; set; }
}

public class RegisterViewModel
{
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Error { get; set; }
}
