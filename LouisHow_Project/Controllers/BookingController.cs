using LouisHow_Project.Data;
using LouisHow_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LouisHow_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public BookingController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Post(Bookings bookings)
        {
            // Get the logged-in user's username
            string loggedInUsername = User.Identity.Name;

            // Populate the BookedBy field with the username
            bookings.BookedBy = loggedInUsername;

            // Set the status to Pending
            bookings.Status = BookingStatus.Pending;

            _context.Bookings.Add(bookings);
            _context.SaveChanges();

            return CreatedAtAction("GetAllUser", new { id = bookings.BookingID }, bookings);
        }
        [HttpGet]
        public IActionResult GetAllUser()
        {
            // Get the logged-in user's username
            string loggedInUsername = User.Identity.Name;

            // Retrieve bookings that were booked by the logged-in user
            var bookingsByUser = _context.Bookings
                .Where(b => b.BookedBy == loggedInUsername)
                .ToList();

            return Ok(bookingsByUser);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var bookings = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (bookings == null)
                return Problem(detail: "Booking with id " + id + "is not found.", statusCode: 404);

            // Check if the booking was created by the logged-in user
            if (bookings.BookedBy != User.Identity.Name)
                return Forbid(); // User is not allowed to access this booking

            return Ok(bookings);
        }

        [HttpGet("{fromDate}/{toDate}")]
        public IActionResult GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            // Get the logged-in user's username
            string loggedInUsername = User.Identity.Name;

            // Adjust the toDate to include the entire day
            toDate = toDate.AddDays(1).AddTicks(-1);

            var bookingsInRange = _context.Bookings
                .Where(b => b.BookingDateFrom >= fromDate && b.BookingDateTo <= toDate && b.BookedBy == loggedInUsername)
                .ToList();

            return Ok(bookingsInRange);
        }

        [HttpGet]
        public IActionResult GetByStatus(BookingStatus status)
        {
            // Get the logged-in user's username
            string loggedInUsername = User.Identity.Name;

            var bookingsByStatus = _context.Bookings
                .Where(b => b.Status == status && b.BookedBy == loggedInUsername)
                .ToList();

            return Ok(bookingsByStatus);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with id " + id + " is not found.");

            // Check if the booking was created by the logged-in user
            if (entity.BookedBy != User.Identity.Name)
                return Forbid(); // User is not allowed to delete this booking

            _context.Bookings.Remove(entity);
            _context.SaveChanges();

            return Ok(entity);
        }


        [HttpPut("{id}")]
        public IActionResult Put(int id, Bookings updatedBookings)
        {
            var entity = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking id " + id + " is not found.", statusCode: 404);

            // Check if the booking was created by the logged-in user
            if (entity.BookedBy != User.Identity.Name)
                return Forbid(); // User is not allowed to update this booking

            // Check if the status is "Confirmed" or "Cancelled"
            if (entity.Status == BookingStatus.Confirmed || entity.Status == BookingStatus.Cancelled)
                return BadRequest("Cannot update a booking with a status of 'Confirmed' or 'Cancelled'.");

            // Check if the status is "Pending" to allow date and facility description changes
            if (entity.Status == BookingStatus.Pending)
            {
                // Update the date and facility description
                entity.BookingDateFrom = updatedBookings.BookingDateFrom;
                entity.BookingDateTo = updatedBookings.BookingDateTo;
                entity.FacilityDescription = updatedBookings.FacilityDescription;
            }

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
        [HttpPost]
        public IActionResult Post(Bookings bookings)
        {
            // Set the status to Pending
            bookings.Status = BookingStatus.Pending;

            _context.Bookings.Add(bookings);
            _context.SaveChanges();
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

            return CreatedAtAction("GetAllUser", new { id = bookings.BookingID }, bookings);
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

            return Ok(bookingsByUser);
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
            entity.FacilityDescription = bookings.FacilityDescription;
            entity.Status = bookings.Status;

            _context.SaveChanges();
            return Ok(entity);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with id " + id + " is not found.");

            _context.Bookings.Remove(entity);
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