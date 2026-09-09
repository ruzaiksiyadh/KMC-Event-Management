using KMC.Api.Models;

namespace KMC.Api.Services;

public interface ITokenService
{
    (string token, DateTime expiresAt) CreateToken(ApplicationUser user);
}
