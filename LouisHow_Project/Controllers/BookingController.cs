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
            _context.Bookings.Add(bookings);
            _context.SaveChanges();

            return CreatedAtAction("GetAll", new { id = bookings.BookingID }, bookings);
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

            return Ok("All bookings cleared.");
        }

        [HttpPut]
        public IActionResult Put(int? id, Bookings bookings)
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
    }
}
