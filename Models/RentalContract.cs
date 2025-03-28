using System.Text.Json.Serialization;

public class RentalContract

{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; }
    
 [JsonPropertyName("startDate")]
    [JsonConverter(typeof(ThaiDateTimeConverter))]
    public DateTime StartDate { get; set; }
    
    [JsonPropertyName("endDate")]
    [JsonConverter(typeof(ThaiDateTimeConverter))]
    public DateTime EndDate { get; set; }
}