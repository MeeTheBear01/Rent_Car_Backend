using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly string _jsonFilePath = "vehicles.json";

        // เพิ่ม JSON Options สำหรับการ Serialize และ Deserialize
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // อ่านข้อมูลจาก JSON
        private List<Vehicle> ReadJsonData()
        {
            // เตรียมข้อมูลเริ่มต้นถ้าไฟล์ยังไม่มี
            if (!System.IO.File.Exists(_jsonFilePath))
            {
                var initialData = new List<Vehicle>
            {
                new Vehicle
                {
                    Id = 1,
                    LicensePlate = "test",
                    Brand = "Toyota",
                    Model = "Fortuner",
                    RentalContract = new RentalContract
                    {
                        Id = 101,
                        CustomerName = "สมพงษ์  สมศรี",
                        StartDate = new DateTime(2025, 3, 1),
                        EndDate = new DateTime(2025, 9, 1)
                    }
                },
                new Vehicle
                {
                    Id = 2,
                    LicensePlate = "test",
                    Brand = "Honda",
                    Model = "Civic",
                    RentalContract = new RentalContract
                    {
                        Id = 102,
                        CustomerName = "Jane Smith",
                        StartDate = new DateTime(2024, 5, 15),
                        EndDate = new DateTime(2024, 11, 15)
                    }
                }
            };

                WriteJsonData(initialData);
                return initialData;
            }

            var jsonData = System.IO.File.ReadAllText(_jsonFilePath);
            return JsonSerializer.Deserialize<List<Vehicle>>(jsonData, _jsonOptions)
                   ?? new List<Vehicle>();
        }

        // บันทึกข้อมูลลง JSON
        private void WriteJsonData(List<Vehicle> vehicles)
        {
            var jsonData = JsonSerializer.Serialize(vehicles, _jsonOptions);
            System.IO.File.WriteAllText(_jsonFilePath, jsonData);
        }

        // GET: api/vehicles
        [HttpGet]
        public ActionResult<IEnumerable<Vehicle>> GetVehicles()
        {
            var vehicles = ReadJsonData();
            return Ok(vehicles);
        }

        // โมเดลสำหรับรับข้อมูลจาก body
        public class VehicleFilterRequest
        {
            public string CustomerName { get; set; }
            public string StartDateStr { get; set; }
            public string EndDateStr { get; set; }
        }

        [HttpPost("search")]
        public ActionResult<IEnumerable<Vehicle>> GetVehiclesByFilter([FromBody] VehicleFilterRequest request)
        {
            var vehicles = ReadJsonData(); // อ่านข้อมูลจากไฟล์หรือฐานข้อมูล

            // สร้างตัวแปรเพื่อเก็บข้อมูลที่กรองแล้ว
            var filteredVehicles = vehicles.AsQueryable();

            // ตรวจสอบ customerName ว่ามีหรือไม่
            if (!string.IsNullOrEmpty(request.CustomerName))
            {
                filteredVehicles = filteredVehicles.Where(v => v.RentalContract.CustomerName.Contains(request.CustomerName, StringComparison.OrdinalIgnoreCase));
            }

            // ตรวจสอบ startDate และ endDate
            if (!string.IsNullOrEmpty(request.StartDateStr) && !string.IsNullOrEmpty(request.EndDateStr))
            {
                // แปลง startDate และ endDate เป็น DateTime
                DateTime startDate = DateTime.TryParse(request.StartDateStr, out var parsedStartDate) ? parsedStartDate : DateTime.MinValue;
                DateTime endDate = DateTime.TryParse(request.EndDateStr, out var parsedEndDate) ? parsedEndDate : DateTime.MaxValue;

                // กรองข้อมูลตามวันที่
                filteredVehicles = filteredVehicles.Where(v =>
                    v.RentalContract.StartDate >= startDate &&
                    v.RentalContract.EndDate <= endDate);
            }

            // ส่งผลลัพธ์ที่กรองแล้ว
            return Ok(filteredVehicles.ToList());
        }
    }
}