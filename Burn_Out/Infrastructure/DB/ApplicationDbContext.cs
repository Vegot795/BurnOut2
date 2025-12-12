using Core.Models;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<HallModel> Halls { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<HallModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HallName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Capacity).IsRequired();
        });
    }
}
