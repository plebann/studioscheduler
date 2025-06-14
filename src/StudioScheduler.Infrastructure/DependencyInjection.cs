using Microsoft.Extensions.DependencyInjection;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Infrastructure.MockRepositories.Implementations;
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
}
