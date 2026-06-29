using System.ComponentModel.DataAnnotations.Schema;

[Table("car_types")]
public class VehicleType
{
    public int id { get; set; }
    public required string type_name { get; set; }
    public required int price_per_day { get; set; }
}