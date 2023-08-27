using LouisHow_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace LouisHow_Project.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base (options)
        {
        }
        public DbSet<Bookings>? Bookings { get; set; }
    }
}
