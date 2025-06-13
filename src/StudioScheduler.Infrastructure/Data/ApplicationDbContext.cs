using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<DanceClass> DanceClasses { get; set; }
    public DbSet<Pass> Passes { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Location relationships
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Location)
            .WithMany(l => l.Rooms)
            .HasForeignKey(r => r.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Location)
            .WithMany(l => l.Schedules)
            .HasForeignKey(s => s.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Room relationships
        modelBuilder.Entity<DanceClass>()
            .HasOne(c => c.Room)
            .WithMany(r => r.Classes)
            .HasForeignKey(c => c.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // DanceClass relationships
        modelBuilder.Entity<DanceClass>()
            .HasOne(c => c.Instructor)
            .WithMany()
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.DanceClass)
            .WithMany(c => c.Schedules)
            .HasForeignKey(s => s.DanceClassId)
            .OnDelete(DeleteBehavior.Restrict);

        // Existing relationships
        modelBuilder.Entity<Reservation>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne<Schedule>()
            .WithMany(s => s.Reservations)
            .HasForeignKey(r => r.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pass>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
