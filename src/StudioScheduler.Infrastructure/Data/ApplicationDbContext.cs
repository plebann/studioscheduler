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
    public DbSet<Student> Students { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<DanceClass> DanceClasses { get; set; }
    public DbSet<Pass> Passes { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

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
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Room)
            .WithMany(r => r.Schedules)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Instructor relationships
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Instructor)
            .WithMany()
            .HasForeignKey(s => s.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        // DanceClass relationships
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.DanceClass)
            .WithMany(c => c.Schedules)
            .HasForeignKey(s => s.DanceClassId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pass>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Student-Pass relationship (Student inherits from User)
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Passes)
            .WithOne()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Student>()
            .HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Student>()
            .HasMany(s => s.AttendanceRecords)
            .WithOne(a => a.Student)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enrollment relationships
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Schedule)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Attendance relationships
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Schedule)
            .WithMany()
            .HasForeignKey(a => a.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Pass)
            .WithMany()
            .HasForeignKey(a => a.PassUsed)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
