using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalaryCalculator.Models;

namespace SalaryCalculator.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
		public DbSet<Shift> Shifts => Set<Shift>();
		public DbSet<PaySettings> PaySettings => Set<PaySettings>();
	}
}
