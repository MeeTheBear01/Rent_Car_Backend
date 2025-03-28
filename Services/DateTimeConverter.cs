using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

public class ThaiDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] _supportedFormats = new[]
    {
        "yyyy-MM-ddTHH:mm:ss.fffZ", // ISO 8601 with milliseconds
        "yyyy-MM-ddTHH:mm:ssZ",     // ISO 8601 without milliseconds
        "yyyy-MM-dd",               // Simple date
        "yyyy-MM-ddTHH:mm:ss",      // Date with time
        "dd/MM/yyyy",               // Thai date format
        "MM/dd/yyyy"                // US date format
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? dateString = reader.GetString();
        
        if (string.IsNullOrEmpty(dateString))
        {
            return default;
        }

        foreach (var format in _supportedFormats)
        {
            if (DateTime.TryParseExact(
                dateString, 
                format, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.RoundtripKind, 
                out DateTime result))
            {
                // Convert UTC to local time if needed
                return result.Kind == DateTimeKind.Utc 
                    ? result.ToLocalTime() 
                    : result;
            }
        }

        // Fallback to TryParse for more flexible parsing
        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, out DateTime fallbackResult))
        {
            return fallbackResult;
        }

        throw new JsonException($"Unable to convert \"{dateString}\" to DateTime.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Ensure the datetime is in UTC when writing
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
    }
}