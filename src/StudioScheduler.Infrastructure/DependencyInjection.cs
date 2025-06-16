using Microsoft.Extensions.DependencyInjection;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Infrastructure.Repositories;
using StudioScheduler.Infrastructure.Services;

namespace StudioScheduler.Infrastructure;

public static class DependencyInjection
{
    // Note: Mock repositories have been removed in favor of Entity Framework repositories with data seeding
    // The JSON mock data is now loaded into the EF database via DataSeedingService

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
        services.AddScoped<IPassRepository, PassRepository>();

        // Register services
        services.AddScoped<IDanceClassService, DanceClassService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IClassAttendanceService, ClassAttendanceService>();
        services.AddScoped<IPassService, PassService>();

        // Register data seeding service
        services.AddScoped<DataSeedingService>();

        return services;
    }
}
