using System.ComponentModel.DataAnnotations.Schema;

[Table("adminusers")]
public class AdminUsers
{
    public int id { get; set; }
    public required string email { get; set; }
    public required bool isactive { get; set; }
    public required DateTime createdat { get; set; }
}