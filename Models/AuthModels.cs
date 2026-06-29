using System.Text.Json.Serialization;

public class GoogleLoginRequest
{
    private string _credential = string.Empty;

    [JsonPropertyName("credential")]
    public string Credential { get => _credential; set => _credential = value ?? string.Empty; }

    [JsonPropertyName("id_token")]
    public string Id_Token { set => _credential = value ?? string.Empty; }

    [JsonPropertyName("idToken")]
    public string IdToken { set => _credential = value ?? string.Empty; }
}

public class AuthUser
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("picture")]
    public string Picture { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    [JsonPropertyName("contractId")]
    public string ContractId { get; set; } = string.Empty;
}

public class AuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public AuthUser User { get; set; } = new();
}

public class BookingRequest
{
    [JsonPropertyName("vehicleId")]
    public int VehicleId { get; set; }

    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonPropertyName("pickupLocation")]
    public string PickupLocation { get; set; } = string.Empty;

    [JsonPropertyName("startDateStr")]
    public string StartDateStr { get; set; } = string.Empty;

    [JsonPropertyName("endDateStr")]
    public string EndDateStr { get; set; } = string.Empty;
}
