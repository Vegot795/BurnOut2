using Core.Models;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<HallModel> Halls { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<HallReservation> HallReservations { get; set; }
    public DbSet<TerminsModel> Termins { get; set; }

}
