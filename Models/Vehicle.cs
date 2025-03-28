using System.Text.Json.Serialization;

public class Vehicle
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("licensePlate")]
    public string LicensePlate { get; set; }
    
    [JsonPropertyName("brand")]
    public string Brand { get; set; }
    
    [JsonPropertyName("model")]
    public string Model { get; set; }
    
    [JsonPropertyName("rentalContract")]
    public RentalContract RentalContract { get; set; }
}