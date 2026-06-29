using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()

    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AuthService(IConfiguration configuration, HttpClient httpClient, AppDbContext context)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _context = context;
    }

    public async Task<AuthResponse> AuthenticateGoogleAsync(string credential)
    {
        if (string.IsNullOrWhiteSpace(credential))
        {
            throw new InvalidOperationException("Google credential is required.");
        }

        var clientId = _configuration["Auth:GoogleClientId"];
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new InvalidOperationException("Google client ID is not configured.");
        }

        var tokenInfoUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={Uri.EscapeDataString(credential)}";
        using var response = await _httpClient.GetAsync(tokenInfoUrl);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Google credential is invalid.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var googleUser = JsonSerializer.Deserialize<GoogleTokenInfo>(json, _jsonOptions)
            ?? throw new InvalidOperationException("Google credential response is invalid.");

        if (!string.Equals(googleUser.Aud, clientId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Google credential audience does not match this app.");
        }

        var requireVerified = _configuration.GetValue<bool>("Auth:RequireEmailVerified", true);
        if (requireVerified && !googleUser.IsEmailVerified())
        {
            throw new InvalidOperationException("Google email is not verified.");
        }

        var user = new AuthUser
        {
            Email = googleUser.Email,
            Name = googleUser.Name,
            Picture = googleUser.Picture,
            Role = await ResolveRole(googleUser.Email)
        };

        return new AuthResponse
        {
            User = user,
            Token = CreateSessionToken(user)
        };
    }

    public AuthUser? GetUserFromRequest(HttpRequest request)
    {
        var authorization = request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return ValidateSessionToken(authorization["Bearer ".Length..].Trim());
    }

    public bool IsAdmin(HttpRequest request)
    {
        return string.Equals(GetUserFromRequest(request)?.Role, "admin", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<string> ResolveRole(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return "user";
        }

        var isAdmin = await _context.AdminUsers
            .AnyAsync(v => v.email.ToLower() == email.ToLower() && v.isactive);

        return isAdmin ? "admin" : "user";
    }

    private string CreateSessionToken(AuthUser user)
    {
        var payload = new SessionPayload
        {
            Email = user.Email,
            Name = user.Name,
            Picture = user.Picture,
            Role = user.Role,
            Exp = DateTimeOffset.UtcNow.AddHours(8).ToUnixTimeSeconds()
        };

        var payloadJson = JsonSerializer.Serialize(payload, _jsonOptions);
        var payloadPart = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        var signaturePart = Sign(payloadPart);
        return $"{payloadPart}.{signaturePart}";
    }

    private AuthUser? ValidateSessionToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 2)
        {
            return null;
        }

        var expectedSignature = Sign(parts[0]);
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(parts[1])))
        {
            return null;
        }

        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
        var payload = JsonSerializer.Deserialize<SessionPayload>(payloadJson, _jsonOptions);
        if (payload == null || payload.Exp < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            return null;
        }

        return new AuthUser
        {
            Email = payload.Email,
            Name = payload.Name,
            Picture = payload.Picture,
            Role = payload.Role
        };
    }

    private string Sign(string payloadPart)
    {
        var secret = _configuration["Auth:TokenSecret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            secret = "development-only-change-this-secret";
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadPart)));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }

    private class GoogleTokenInfo
    {
        [JsonPropertyName("aud")]
        public string Aud { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("email_verified")]
        public JsonElement EmailVerified { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("contractId")]
        public string ContractId { get; set; } = string.Empty;

        public bool IsEmailVerified()
        {
            if (EmailVerified.ValueKind == JsonValueKind.True) return true;
            if (EmailVerified.ValueKind == JsonValueKind.False) return false;
            if (EmailVerified.ValueKind == JsonValueKind.String)
            {
                var s = EmailVerified.GetString();
                return string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }

    private class SessionPayload
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public long Exp { get; set; }
    }
}
