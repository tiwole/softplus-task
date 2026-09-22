using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Core.Entities;
using Microsoft.Extensions.Options;

namespace Infrastructure.Security;

public class JwtTokenService(IOptions<JwtOptions> options)
{
    public (string Token, DateTime ExpiresAtUtc) Create(ApplicationUser user)
    {
        var jwtOptions = options.Value;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiresMinutes);
        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };
        var payload = new Dictionary<string, object>
        {
            ["sub"] = user.Id.ToString(),
            ["email"] = user.Email,
            ["iss"] = jwtOptions.Issuer,
            ["aud"] = jwtOptions.Audience,
            ["iat"] = ToUnixTimeSeconds(DateTime.UtcNow),
            ["exp"] = ToUnixTimeSeconds(expiresAtUtc)
        };

        var encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        var unsignedToken = $"{encodedHeader}.{encodedPayload}";
        var signature = CreateSignature(unsignedToken, jwtOptions.Secret);

        return ($"{unsignedToken}.{signature}", expiresAtUtc);
    }

    public ClaimsPrincipal? Validate(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return null;
            }

            var unsignedToken = $"{parts[0]}.{parts[1]}";
            var expectedSignature = CreateSignature(unsignedToken, options.Value.Secret);
            if (!FixedTimeEquals(parts[2], expectedSignature))
            {
                return null;
            }

            var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Base64UrlDecode(parts[1]));
            if (payload is null)
            {
                return null;
            }

            if (!ClaimEquals(payload, "iss", options.Value.Issuer) ||
                !ClaimEquals(payload, "aud", options.Value.Audience) ||
                !payload.TryGetValue("sub", out var sub) ||
                !payload.TryGetValue("exp", out var exp))
            {
                return null;
            }

            var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64()).UtcDateTime;
            if (expiresAtUtc <= DateTime.UtcNow)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new("sub", sub.GetString() ?? string.Empty)
            };

            if (payload.TryGetValue("email", out var email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email.GetString() ?? string.Empty));
            }

            var identity = new ClaimsIdentity(claims, "Bearer");
            return new ClaimsPrincipal(identity);
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private static string CreateSignature(string unsignedToken, string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(secretBytes);
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));

        return Base64UrlEncode(signatureBytes);
    }

    private static bool ClaimEquals(IReadOnlyDictionary<string, JsonElement> payload, string name, string value)
    {
        return payload.TryGetValue(name, out var claim) &&
               string.Equals(claim.GetString(), value, StringComparison.Ordinal);
    }

    private static long ToUnixTimeSeconds(DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');

        return Convert.FromBase64String(base64);
    }

    private static bool FixedTimeEquals(string value, string expected)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(value),
            Encoding.UTF8.GetBytes(expected));
    }
}
