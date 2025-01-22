using Microsoft.EntityFrameworkCore;
using PaymentService.Model;

namespace PaymentService.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; }
        
    }
}
