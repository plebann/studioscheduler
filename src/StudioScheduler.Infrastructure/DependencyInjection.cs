using Microsoft.Extensions.DependencyInjection;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Infrastructure.MockRepositories.Implementations;
using StudioScheduler.Infrastructure.Repositories;
using StudioScheduler.Infrastructure.Services;

namespace StudioScheduler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMockRepositories(this IServiceCollection services)
    {
        // Register repositories
        services.AddSingleton<ILocationRepository, MockLocationRepository>();
        services.AddSingleton<IRoomRepository, MockRoomRepository>();
        services.AddSingleton<IDanceClassRepository, MockDanceClassRepository>();
        services.AddSingleton<IScheduleRepository, MockScheduleRepository>();
        services.AddSingleton<IStudentRepository, MockStudentRepository>();
        services.AddSingleton<IEnrollmentRepository, MockEnrollmentRepository>();
        services.AddSingleton<IAttendanceRepository, MockAttendanceRepository>();

        // Register services
        services.AddScoped<IDanceClassService, DanceClassService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IClassAttendanceService, ClassAttendanceService>();

        return services;
    }

    public static IServiceCollection AddEfRepositories(this IServiceCollection services)
    {
        // Register EF repositories
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IDanceClassRepository, DanceClassRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();

        // Register services
        services.AddScoped<IDanceClassService, DanceClassService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IClassAttendanceService, ClassAttendanceService>();

        // Register data seeding service
        services.AddScoped<DataSeedingService>();

        return services;
    }
}
