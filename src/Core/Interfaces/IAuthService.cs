using Shared.Auth;

namespace Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
}
