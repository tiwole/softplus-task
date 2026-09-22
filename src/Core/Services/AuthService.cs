using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Auth;

namespace Core.Services;

public class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasher<ApplicationUser> passwordHasher) : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) is PasswordVerificationResult.Failed )
            throw new AuthException("Invalid email or password");

        var token = tokenService.Create(user);
        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc
        };
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var exists = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (exists)
            throw new AuthException("User with that email already exists");

        var userToHash = new ApplicationUser { Email = request.Email };
        var passwordHash = passwordHasher.HashPassword(userToHash, request.Password);
        var user = await userRepository.CreateAsync(
            request.Email,
            passwordHash,
            cancellationToken);
        var token = tokenService.Create(user);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc
        };
    }
}
