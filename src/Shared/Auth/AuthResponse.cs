namespace Shared.Auth;

public class AuthResponse
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; } = DateTime.MinValue;
}
