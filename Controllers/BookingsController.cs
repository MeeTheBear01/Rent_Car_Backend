using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly AuthService _authService;

        public BookingsController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public ActionResult CreateBooking([FromBody] BookingRequest request)
        {
            var user = _authService.GetUserFromRequest(Request);
            if (user == null)
            {
                return Unauthorized(new { message = "Login is required before booking a car." });
            }

            return Ok(new
            {
                message = "Booking request received.",
                bookedBy = user.Email,
                role = user.Role,
                booking = request
            });
        }
    }
}
