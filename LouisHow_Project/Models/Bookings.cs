using System.ComponentModel.DataAnnotations;

namespace LouisHow_Project.Models
{
    public class Bookings
    {
        [Key]
        public int BookingID { get; set; }
        public string? FacilityDescription { get; set; }
        public DateTime BookingDateFrom { get; set; }
        public DateTime BookingDateTo { get; set; } 
        public string? BookedBy { get; set; }
        
        [EnumDataType(typeof(BookingStatus), ErrorMessage = "Invalid status value")]
        public BookingStatus Status { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}