using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;
using StudioScheduler.Core.Services;

namespace StudioScheduler.Infrastructure.Services;

public class PassService : IPassService
{
    private readonly IPassRepository _passRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public PassService(IPassRepository passRepository, IAttendanceRepository attendanceRepository, IEnrollmentRepository enrollmentRepository, IScheduleRepository scheduleRepository)
    {
        _passRepository = passRepository;
        _attendanceRepository = attendanceRepository;
        _enrollmentRepository = enrollmentRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Pass?> GetByIdAsync(Guid id)
    {
        return await _passRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Pass>> GetAllAsync()
    {
        return await _passRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Pass>> GetByUserIdAsync(Guid userId)
    {
        return await _passRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Pass>> GetActivePassesAsync()
    {
        return await _passRepository.GetActivePassesAsync();
    }

    public async Task<IEnumerable<Pass>> GetExpiredPassesAsync()
    {
        return await _passRepository.GetExpiredPassesAsync();
    }

    public async Task<Pass?> GetCurrentActivePassForUserAsync(Guid userId)
    {
        return await _passRepository.GetCurrentActivePassForUserAsync(userId);
    }

    public async Task<IEnumerable<Pass>> GetPassesExpiringInDaysAsync(int days)
    {
        return await _passRepository.GetPassesExpiringInDaysAsync(days);
    }

    public async Task<Pass> CreatePassAsync(Pass pass)
    {
        // Validate pass creation logic
        ValidatePassDates(pass);
        ValidatePassType(pass);

        return await _passRepository.AddAsync(pass);
    }

    public async Task<Pass> UpdatePassAsync(Pass pass)
    {
        var existingPass = await _passRepository.GetByIdAsync(pass.Id);
        if (existingPass == null)
            throw new ArgumentException($"Pass with ID {pass.Id} not found");

        ValidatePassDates(pass);
        
        return await _passRepository.UpdateAsync(pass);
    }

    public async Task DeletePassAsync(Guid id)
    {
        var existingPass = await _passRepository.GetByIdAsync(id);
        if (existingPass == null)
            throw new ArgumentException($"Pass with ID {id} not found");

        await _passRepository.DeleteAsync(id);
    }

    public async Task<bool> CanUsePassForClassAsync(Guid passId, Guid scheduleId, DateTime classDate)
    {
        var pass = await _passRepository.GetByIdAsync(passId);
        if (pass == null) return false;

        // Get attendance history for this pass
        var attendances = await _attendanceRepository.GetByPassIdAsync(passId);
        
        return pass.CanUseForClass(classDate, attendances);
    }

    public async Task<Pass> ExtendPassAsync(Guid passId, int additionalDays)
    {
        var pass = await _passRepository.GetByIdAsync(passId);
        if (pass == null)
            throw new ArgumentException($"Pass with ID {passId} not found");

        pass.EndDate = pass.EndDate.AddDays(additionalDays);
        pass.UpdatedAt = DateTime.UtcNow;

        return await _passRepository.UpdateAsync(pass);
    }

    public async Task<Pass> ActivatePassAsync(Guid passId)
    {
        var pass = await _passRepository.GetByIdAsync(passId);
        if (pass == null)
            throw new ArgumentException($"Pass with ID {passId} not found");

        pass.IsActive = true;
        pass.UpdatedAt = DateTime.UtcNow;

        return await _passRepository.UpdateAsync(pass);
    }

    public async Task<Pass> DeactivatePassAsync(Guid passId)
    {
        var pass = await _passRepository.GetByIdAsync(passId);
        if (pass == null)
            throw new ArgumentException($"Pass with ID {passId} not found");

        pass.IsActive = false;
        pass.UpdatedAt = DateTime.UtcNow;

        return await _passRepository.UpdateAsync(pass);
    }

    public async Task<IEnumerable<Pass>> GetPassesByTypeAsync(string passType)
    {
        return await _passRepository.GetPassesByTypeAsync(passType);
    }

    public async Task<int> GetPassUsageStatsAsync(Guid passId, DateTime? fromDate = null)
    {
        var pass = await _passRepository.GetByIdAsync(passId);
        if (pass == null) return 0;

        var attendances = await _attendanceRepository.GetByPassIdAsync(passId);
        
        if (fromDate.HasValue)
        {
            attendances = attendances.Where(a => a.ClassDate >= fromDate.Value);
        }

        return pass.GetUsedClassesCount(attendances);
    }

    public async Task<bool> IsPassValidForDateAsync(Guid passId, DateTime date)
    {
        var pass = await _passRepository.GetByIdAsync(passId);
        if (pass == null) return false;

        return pass.IsValidOn(date);
    }

    public async Task<Pass> PurchasePassAsync(Guid studentId, PassType passType, DateTime startDate, List<Guid> selectedScheduleIds)
    {
        // Validate input
        if (!PassConfigurationService.IsMonthlyPass(passType))
            throw new ArgumentException($"Pass type {passType} is not supported for purchase");

        if (selectedScheduleIds.Count != PassConfigurationService.GetClassesPerWeek(passType))
            throw new ArgumentException($"Must select exactly {PassConfigurationService.GetClassesPerWeek(passType)} classes for {passType}");

        // Validate start date aligns with selected schedule days
        await ValidateStartDateWithSchedules(startDate, selectedScheduleIds);

        // Create the pass
        var pass = new Pass
        {
            Id = Guid.NewGuid(),
            UserId = studentId,
            Type = passType,
            StartDate = startDate,
            EndDate = startDate.AddDays(28), // 28-day validity
            ClassesPerWeek = PassConfigurationService.GetClassesPerWeek(passType),
            TotalClasses = PassConfigurationService.GetTotalClasses(passType),
            IsActive = true
        };

        var enrollments = new List<Enrollment>();
        
        foreach (var scheduleId in selectedScheduleIds)
        {
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ScheduleId = scheduleId,
                EnrolledDate = startDate, // Use actual start date, not current time
                IsActive = true
            };
            
            var createdEnrollment = await _enrollmentRepository.CreateOrReactivateAsync(enrollment);
            enrollments.Add(createdEnrollment);
        }

        var createdPass = await _passRepository.AddAsync(pass);

        return createdPass;
    }

    // Private helper methods
    private static void ValidatePassDates(Pass pass)
    {
        if (pass.EndDate <= pass.StartDate)
            throw new ArgumentException("End date must be after start date");

        if (pass.StartDate < DateTime.Today.AddDays(-1))
            throw new ArgumentException("Start date cannot be in the past");
    }

    private static void ValidatePassType(Pass pass)
    {
        // Validate SalsaMe-specific business rules
        switch (pass.Type)
        {
            case PassType.SingleClass:
                if (pass.TotalClasses != 1)
                    throw new ArgumentException("Single class pass must have exactly 1 total class");
                if (pass.ClassesPerWeek != 1)
                    throw new ArgumentException("Single class pass must have exactly 1 class per week");
                break;

            case PassType.Flexi4Classes:
                if (pass.TotalClasses != 4)
                    throw new ArgumentException("Flexi 4 class pass must have exactly 4 total classes");
                if (pass.ClassesPerWeek != 1)
                    throw new ArgumentException("Flexi 4 class pass allows 1 class per week");
                break;

            case PassType.Flexi8Classes:
                if (pass.TotalClasses != 8)
                    throw new ArgumentException("Flexi 8 class pass must have exactly 8 total classes");
                if (pass.ClassesPerWeek != 2)
                    throw new ArgumentException("Flexi 8 class pass allows 2 classes per week");
                break;

            case PassType.FullPass:
                // No restrictions on total classes for FullPass
                break;

            default:
                // Regular monthly passes
                var expectedTotalClasses = pass.ClassesPerWeek * 4; // 28-day periods = 4 weeks
                if (pass.TotalClasses != expectedTotalClasses)
                    throw new ArgumentException($"{pass.Type} should have {expectedTotalClasses} total classes (4 weeks × {pass.ClassesPerWeek} classes/week)");
                break;
        }

        // Validate 28-day validity period for SalsaMe passes
        if (pass.Type != PassType.SingleClass && pass.Type != PassType.FullPass)
        {
            var validityPeriod = (pass.EndDate - pass.StartDate).Days + 1;
            if (validityPeriod != 28)
                throw new ArgumentException($"SalsaMe passes must have exactly 28-day validity period, got {validityPeriod} days");
        }
    }

    private async Task ValidateStartDateWithSchedules(DateTime startDate, List<Guid> selectedScheduleIds)
    {
        var selectedDays = new List<DayOfWeek>();
        
        foreach (var scheduleId in selectedScheduleIds)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new ArgumentException($"Schedule with ID {scheduleId} not found");
            
            selectedDays.Add(schedule.DayOfWeek);
        }

        // Start date must match one of the selected class days
        if (!selectedDays.Contains(startDate.DayOfWeek))
        {
            var selectedDayNames = string.Join(", ", selectedDays.Select(d => d.ToString()));
            throw new ArgumentException($"Start date ({startDate:yyyy-MM-dd}) is on {startDate.DayOfWeek}, but selected classes are on: {selectedDayNames}. Start date must match one of the selected class days.");
        }
    }
}
