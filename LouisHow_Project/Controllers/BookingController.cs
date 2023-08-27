using LouisHow_Project.Data;
using LouisHow_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LouisHow_Project.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public BookingController(ApplicationDBContext context)
        {
            _context = context;
        }

        //GET: api/Bookings
        [HttpGet]
        public IActionResult GetAll() 
        {
            return Ok(_context.Bookings);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) 
        {
            var bookings = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (bookings == null)
                return Problem(detail: "Booking with id " + id + "is not found.", statusCode: 404);
            return Ok(bookings); ;
        }
        [HttpGet("byDateRange")]
        public IActionResult GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var bookingsInRange = _context.Bookings
                .Where(b => b.BookingDateFrom >= fromDate && b.BookingDateTo <= toDate)
                .ToList();

            return Ok(bookingsInRange);
        }
        [HttpGet("byBookedBy")]
        public IActionResult GetByBookedBy(string bookedBy)
        {
            var bookingsByUser = _context.Bookings
                .Where(b => b.BookedBy == bookedBy)
                .ToList();

            return Ok(bookingsByUser);
        }
        [HttpGet("byStatus")]
        public IActionResult GetByStatus(BookingStatus status)
        {
            var bookingsByStatus = _context.Bookings
                .Where(b => b.Status == status)
                .ToList();

            return Ok(bookingsByStatus);
        }

        [HttpPost]
        public IActionResult Post(Bookings bookings) 
        {
            var entity = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with id " + id + " is not found.");

            // Check if the booking was created by the logged-in user
            if (entity.BookedBy != User.Identity.Name)
                return Forbid(); // User is not allowed to delete this booking

            // Check if the status is not "Pending"
            if (entity.Status != BookingStatus.Pending)
            {
                // If the status is not "Pending," change it to "Cancelled"
                entity.Status = BookingStatus.Cancelled;
                _context.SaveChanges();
                return Ok(entity);
            }

            _context.Bookings.Remove(entity);
            _context.SaveChanges();

            return Ok(entity);
        }
        // DELETE: api/Booking
        [HttpDelete]
        public IActionResult ClearBookings()
        {
            var allBookings = _context.Bookings.ToList();

            if (allBookings.Count == 0)
            {
                return NotFound("No bookings found.");
            }

            _context.Bookings.RemoveRange(allBookings);
            _context.SaveChanges();
            return Ok(entity);
        }

    }

    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AdminController(ApplicationDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public IActionResult GetAllAdmin()
        {
            return Ok(_context.Bookings);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("{bookedBy}")]
        public IActionResult BookedBy(string bookedBy)
        {
            var bookingsByUser = _context.Bookings
                .Where(b => b.BookedBy == bookedBy)
                .ToList();

            return Ok("All bookings cleared.");
        }
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public IActionResult GetByStatus(BookingStatus status)
        {
            var bookingsByStatus = _context.Bookings
                .Where(b => b.Status == status)
                .ToList();

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public IActionResult Put(int id, Bookings bookings)
        {
            var entity = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking id " + id + " is not found.", statusCode: 404);

            entity.FacilityDescription = bookings.FacilityDescription;
            entity.BookingDateFrom = bookings.BookingDateFrom;
            entity.BookingDateTo = bookings.BookingDateTo;
            entity.Status = bookings.Status;

            _context.SaveChanges();
            return Ok(entity);
        }

        [Authorize(Roles = UserRoles.Admin)]
        // DELETE: api/Booking
        [HttpDelete]
        public IActionResult ClearBookings()
        {
            var allBookings = _context.Bookings.ToList();

            if (allBookings.Count == 0)
            {
                return NotFound("No bookings found.");
            }

            _context.Bookings.RemoveRange(allBookings);
            _context.SaveChanges();

            return Ok("All bookings cleared.");
        }
    }
}
