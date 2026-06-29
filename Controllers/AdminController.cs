using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly string _jsonFilePath = "vehicles.json";
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public AdminController(AuthService authService)
        {
            _authService = authService;
        }

        //[HttpGet("rentals")]
        //public ActionResult<IEnumerable<Vehicle>> GetRentedVehicles()
        //{
        //    if (!_authService.IsAdmin(Request))
        //    {
        //        return Forbid();
        //    }

        //    if (!System.IO.File.Exists(_jsonFilePath))
        //    {
        //        return Ok(Array.Empty<Vehicle>());
        //    }

        //    var jsonData = System.IO.File.ReadAllText(_jsonFilePath);
        //    var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(jsonData, _jsonOptions) ?? new List<Vehicle>();
        //    return Ok(vehicles.Where(vehicle => vehicle.RentalContract != null).ToList());
        //}
    }
}
