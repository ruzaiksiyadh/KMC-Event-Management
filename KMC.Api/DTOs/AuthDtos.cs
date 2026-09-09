namespace KMC.Api.DTOs;

public record RegisterOrganizerDto(string DisplayName, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, DateTime ExpiresAt, string DisplayName, string UserId);
