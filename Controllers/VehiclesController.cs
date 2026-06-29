using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehiclesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            var vehicles = await _context.Vehicles
                                 .Include(v => v.CarType)
                                 .ToListAsync();
            return Ok(vehicles);
        }

        public class VehicleFilterRequest
        {
            public string? CarTypeId { get; set; }
            public string? Brand { get; set; }
            public string? Model { get; set; }
        }

        //[HttpPost("search")]
        //public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehiclesByFilter([FromBody] VehicleFilterRequest request)
        //{
        //    var query = _context.Vehicles.Include(v => v.booking).AsQueryable();

        //    if (!string.IsNullOrEmpty(request.CustomerName))
        //    {
        //        query = query.Where(v => v.RentalContract.CustomerName.Contains(request.CustomerName, StringComparison.OrdinalIgnoreCase));
        //    }

        //    if (!string.IsNullOrEmpty(request.StartDateStr) && !string.IsNullOrEmpty(request.EndDateStr))
        //    {
        //        DateTime startDate = DateTime.TryParse(request.StartDateStr, out var parsedStart) ? parsedStart : DateTime.MinValue;
        //        DateTime endDate = DateTime.TryParse(request.EndDateStr, out var parsedEnd) ? parsedEnd : DateTime.MaxValue;
        //        query = query.Where(v => v.RentalContract.StartDate >= startDate && v.RentalContract.EndDate <= endDate);
        //    }

        //    var filtered = await query.ToListAsync();
        //    return Ok(filtered);
        //}

        //[HttpGet("{contractId}")]
        //public async Task<ActionResult<Vehicle>> GetVehicleByContractId(int contractId)
        //{
        //    var vehicle = await _context.Vehicles
        //        .Include(v => v.RentalContract)
        //        .FirstOrDefaultAsync(v => v.RentalContract.Id == contractId);

        //    if (vehicle == null)
        //        return NotFound($"Vehicle with ContractId {contractId} not found.");

<<<<<<< HEAD
        //    return Ok(vehicle);
        //}
=======
          [HttpGet("search/{contractId}")]
    public ActionResult<Vehicle> GetVehicleByContractId(int contractId)
    {
        var vehicles = ReadJsonData(); // อ่านข้อมูลจากไฟล์หรือฐานข้อมูล

        // ค้นหายานพาหนะที่มี RentalContract.Id ตรงกับ contractId ที่ส่งมา
        var vehicle = vehicles.FirstOrDefault(v => v.RentalContract.Id == contractId);

        if (vehicle == null)
        {
            return NotFound($"Vehicle with ContractId {contractId} not found.");
        }

        return Ok(vehicle);  // ส่งคืนข้อมูลยานพาหนะที่ตรงกับ ContractId
>>>>>>> parent of 07b22e8 (add api query by ID)
    }
}
