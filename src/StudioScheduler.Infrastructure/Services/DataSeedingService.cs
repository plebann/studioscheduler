using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;
using StudioScheduler.Infrastructure.Data;
using System.Text.Json;

namespace StudioScheduler.Infrastructure.Services;

public class DataSeedingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataSeedingService> _logger;

    public DataSeedingService(ApplicationDbContext context, ILogger<DataSeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedDataAsync()
    {
        // Check if database is already seeded
        if (await _context.Locations.AnyAsync())
        {
            _logger.LogInformation("Database already contains data. Skipping seeding.");
            return;
        }

        _logger.LogInformation("Starting data seeding from JSON files...");

        try
        {
            await SeedLocationsAsync();
            await SeedRoomsAsync();
            await SeedDanceClassesAsync();
            await SeedSchedulesAsync();
            await SeedStudentsAsync();
            await SeedEnrollmentsAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data seeding.");
            throw;
        }
    }

    private async Task SeedLocationsAsync()
    {
        var locationsJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", 
            "StudioScheduler.Infrastructure", "MockRepositories", "Data", "locations.json");
        
        if (!File.Exists(locationsJsonPath))
        {
            _logger.LogWarning("Locations JSON file not found at: {Path}", locationsJsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(locationsJsonPath);
        var data = JsonSerializer.Deserialize<LocationsData>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (data?.Locations != null)
        {
            foreach (var location in data.Locations)
            {
                _context.Locations.Add(location);
            }
            _logger.LogInformation("Seeded {Count} locations", data.Locations.Count);
        }
    }

    private async Task SeedRoomsAsync()
    {
        var roomsJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", 
            "StudioScheduler.Infrastructure", "MockRepositories", "Data", "rooms.json");
        
        if (!File.Exists(roomsJsonPath))
        {
            _logger.LogWarning("Rooms JSON file not found at: {Path}", roomsJsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(roomsJsonPath);
        var data = JsonSerializer.Deserialize<RoomsData>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (data?.Rooms != null)
        {
            foreach (var roomData in data.Rooms)
            {
                var equipment = new List<string>();
                if (roomData.Features.HasAirConditioning) equipment.Add("Air Conditioning");
                if (roomData.Features.HasMirrors) equipment.Add("Mirrors");
                if (roomData.Features.HasSpecialFlooring) equipment.Add("Special Flooring");
                if (roomData.Features.HasSoundSystem) equipment.Add("Sound System");
                if (roomData.Features.HasWaterDispenser) equipment.Add("Water Dispenser");

                var room = new Room
                {
                    Id = Guid.Parse(roomData.Id),
                    Name = roomData.Name,
                    Description = roomData.Description,
                    LocationId = Guid.Parse(roomData.LocationId),
                    Capacity = roomData.Capacity,
                    IsActive = roomData.IsActive,
                    Equipment = equipment,
                    CreatedAt = roomData.CreatedAt,
                    UpdatedAt = roomData.UpdatedAt
                };
                _context.Rooms.Add(room);
            }
            _logger.LogInformation("Seeded {Count} rooms", data.Rooms.Count);
        }
    }

    private async Task SeedDanceClassesAsync()
    {
        var classesJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", 
            "StudioScheduler.Infrastructure", "MockRepositories", "Data", "classes.json");
        
        if (!File.Exists(classesJsonPath))
        {
            _logger.LogWarning("Classes JSON file not found at: {Path}", classesJsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(classesJsonPath);
        var data = JsonSerializer.Deserialize<ClassesData>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (data?.Classes != null)
        {
            foreach (var danceClass in data.Classes)
            {
                _context.DanceClasses.Add(danceClass);
            }
            _logger.LogInformation("Seeded {Count} dance classes", data.Classes.Count);
        }
    }

    private async Task SeedSchedulesAsync()
    {
        var schedulesJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", 
            "StudioScheduler.Infrastructure", "MockRepositories", "Data", "schedules.json");
        
        if (!File.Exists(schedulesJsonPath))
        {
            _logger.LogWarning("Schedules JSON file not found at: {Path}", schedulesJsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(schedulesJsonPath);
        var data = JsonSerializer.Deserialize<SchedulesData>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (data?.Schedules != null)
        {
            foreach (var scheduleData in data.Schedules)
            {
                var schedule = new Schedule
                {
                    Id = Guid.Parse(scheduleData.Id),
                    Name = scheduleData.Name,
                    LocationId = Guid.Parse(scheduleData.LocationId),
                    EffectiveFrom = scheduleData.EffectiveFrom,
                    EffectiveTo = scheduleData.EffectiveTo,
                    IsActive = scheduleData.IsActive,
                    DanceClassId = Guid.Parse(scheduleData.DanceClassId),
                    StartTime = scheduleData.StartTime,
                    Duration = scheduleData.Duration,
                    IsRecurring = scheduleData.IsRecurring,
                    RecurrencePattern = scheduleData.RecurrencePattern,
                    RecurrenceEndDate = scheduleData.RecurrenceEndDate,
                    IsCancelled = scheduleData.IsCancelled,
                    CreatedAt = scheduleData.CreatedAt,
                    UpdatedAt = scheduleData.UpdatedAt
                };
                _context.Schedules.Add(schedule);
            }
            _logger.LogInformation("Seeded {Count} schedules", data.Schedules.Count);
        }
    }

    private async Task SeedStudentsAsync()
    {
        var studentsJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", 
            "StudioScheduler.Infrastructure", "MockRepositories", "Data", "students.json");
        
        if (!File.Exists(studentsJsonPath))
        {
            _logger.LogWarning("Students JSON file not found at: {Path}", studentsJsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(studentsJsonPath);
        var data = JsonSerializer.Deserialize<StudentsData>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (data?.Students != null)
        {
            foreach (var studentData in data.Students)
            {
                var student = new Student
                {
                    Id = Guid.Parse(studentData.Id),
                    FirstName = studentData.FirstName,
                    LastName = studentData.LastName,
                    Email = studentData.Email,
                    PasswordHash = "default_hash_for_migration", // Default password for migration
                    Phone = studentData.Phone,
                    DateOfBirth = studentData.DateOfBirth,
                    Gender = studentData.Gender,
                    IsActive = studentData.IsActive,
                    CreatedAt = studentData.CreatedAt,
                    Role = UserRole.Student
                };
                _context.Students.Add(student);

                // Add student's pass if they have one
                if (studentData.CurrentPass != null)
                {
                    var passType = PassType.Monthly; // Default
                    if (Enum.TryParse<PassType>(studentData.CurrentPass.Type, out var parsedPassType))
                    {
                        passType = parsedPassType;
                    }

                    var passId = Guid.NewGuid(); // Default new GUID
                    if (Guid.TryParse(studentData.CurrentPass.Id, out var parsedPassId))
                    {
                        passId = parsedPassId;
                    }

                    var pass = new Pass
                    {
                        Id = passId,
                        UserId = student.Id,
                        Type = passType,
                        StartDate = studentData.CurrentPass.StartDate,
                        EndDate = studentData.CurrentPass.EndDate,
                        TotalClasses = studentData.CurrentPass.TotalClasses,
                        RemainingClasses = studentData.CurrentPass.RemainingClasses,
                        ClassesPerWeek = studentData.CurrentPass.ClassesPerWeek,
                        IsActive = studentData.CurrentPass.IsActive,
                        CreatedAt = studentData.CreatedAt
                    };
                    _context.Passes.Add(pass);
                }
            }
            _logger.LogInformation("Seeded {Count} students", data.Students.Count);
        }
    }

    private async Task SeedEnrollmentsAsync()
    {
        var enrollmentsJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", 
            "StudioScheduler.Infrastructure", "MockRepositories", "Data", "enrollments.json");
        
        if (!File.Exists(enrollmentsJsonPath))
        {
            _logger.LogWarning("Enrollments JSON file not found at: {Path}", enrollmentsJsonPath);
            return;
        }

        var json = await File.ReadAllTextAsync(enrollmentsJsonPath);
        var data = JsonSerializer.Deserialize<EnrollmentsData>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        if (data?.Enrollments != null)
        {
            foreach (var enrollmentData in data.Enrollments)
            {
                var enrollment = new Enrollment
                {
                    Id = Guid.Parse(enrollmentData.Id),
                    StudentId = Guid.Parse(enrollmentData.StudentId),
                    ScheduleId = Guid.Parse(enrollmentData.ScheduleId),
                    EnrolledDate = enrollmentData.EnrolledDate,
                    IsActive = enrollmentData.IsActive,
                    CreatedAt = enrollmentData.EnrolledDate
                };
                _context.Enrollments.Add(enrollment);
            }
            _logger.LogInformation("Seeded {Count} enrollments", data.Enrollments.Count);
        }
    }

    // Data transfer objects for JSON deserialization
    private class LocationsData
    {
        public List<Location> Locations { get; set; } = new();
    }

    private class RoomsData
    {
        public List<RoomJsonData> Rooms { get; set; } = new();
    }

    private class RoomJsonData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LocationId { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public double Area { get; set; }
        public RoomFeatures Features { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class RoomFeatures
    {
        public bool HasAirConditioning { get; set; }
        public bool HasMirrors { get; set; }
        public bool HasSpecialFlooring { get; set; }
        public bool HasSoundSystem { get; set; }
        public bool HasWaterDispenser { get; set; }
    }

    private class ClassesData
    {
        public List<DanceClass> Classes { get; set; } = new();
    }

    private class SchedulesData
    {
        public List<ScheduleJsonData> Schedules { get; set; } = new();
    }

    private class ScheduleJsonData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LocationId { get; set; } = string.Empty;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
        public string DanceClassId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private class StudentsData
    {
        public List<StudentJsonData> Students { get; set; } = new();
    }

    private class StudentJsonData
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public PassJsonData? CurrentPass { get; set; }
    }

    private class PassJsonData
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalClasses { get; set; }
        public int RemainingClasses { get; set; }
        public int ClassesPerWeek { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired { get; set; }
    }

    private class EnrollmentsData
    {
        public List<EnrollmentJsonData> Enrollments { get; set; } = new();
    }

    private class EnrollmentJsonData
    {
        public string Id { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string ScheduleId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EnrolledDate { get; set; }
        public bool IsActive { get; set; }
    }
}
