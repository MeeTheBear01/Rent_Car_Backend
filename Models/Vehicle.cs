using System.ComponentModel.DataAnnotations.Schema;

[Table("cars")]
public class Vehicle
{
    public int id { get; set; }
    public required string brand { get; set; }
    public required string model { get; set; }
    public required string license_plate { get; set; }
    public required string car_status { get; set; }

    [ForeignKey("car_type_id")]
    public virtual VehicleType? CarType { get; set; }
}