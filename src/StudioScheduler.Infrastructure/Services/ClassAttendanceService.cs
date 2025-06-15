using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Models;

namespace StudioScheduler.Infrastructure.Services;

public class ClassAttendanceService : IClassAttendanceService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IDanceClassRepository _danceClassRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IAttendanceRepository _attendanceRepository;

    public ClassAttendanceService(
        IScheduleRepository scheduleRepository,
        IDanceClassRepository danceClassRepository,
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository,
        IAttendanceRepository attendanceRepository)
    {
        _scheduleRepository = scheduleRepository;
        _danceClassRepository = danceClassRepository;
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<Schedule?> GetClassAttendanceAsync(Guid scheduleId)
    {
        return await _scheduleRepository.GetByIdAsync(scheduleId);
    }

    public async Task<Attendance> MarkAttendanceAsync(Guid scheduleId, Guid studentId, bool isPresent, string? notes = null)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new ArgumentException("Student not found", nameof(studentId));

        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null)
            throw new ArgumentException("Schedule not found", nameof(scheduleId));

        // Check if attendance already exists for today
        var existingAttendance = await _attendanceRepository.GetByStudentScheduleAndDateAsync(
            studentId, scheduleId, DateTime.Today);

        if (existingAttendance != null)
        {
            // Update existing attendance
            existingAttendance.WasPresent = isPresent;
            existingAttendance.Notes = notes;
            existingAttendance.UpdatedAt = DateTime.UtcNow;
            await _attendanceRepository.UpdateAsync(existingAttendance);
            return existingAttendance;
        }
        else
        {
            // Create new attendance record
            var newAttendance = new Attendance
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ScheduleId = scheduleId,
                ClassDate = DateTime.Today,
                WasPresent = isPresent,
                Notes = notes,
                PassUsed = student.CurrentPass?.Id,
                PassClassNumber = await CalculatePassClassNumber(studentId, scheduleId)
            };

            await _attendanceRepository.CreateAsync(newAttendance);
            return newAttendance;
        }
    }

    public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 3)
            return new List<Student>();

        var students = await _studentRepository.GetAllAsync();
        return students.Where(s => 
            s.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            s.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            $"{s.FirstName} {s.LastName}".Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Take(10);
    }

    public async Task<IEnumerable<Attendance>> GetAttendanceHistoryAsync(Guid studentId, Guid scheduleId, int weeks = 3)
    {
        var endDate = DateTime.Today;
        var startDate = endDate.AddDays(-weeks * 7);

        var allAttendance = await _attendanceRepository.GetByStudentAndScheduleAsync(studentId, scheduleId);
        return allAttendance.Where(a => a.ClassDate >= startDate && a.ClassDate <= endDate)
            .OrderByDescending(a => a.ClassDate);
    }

    private async Task<int> CalculatePassClassNumber(Guid studentId, Guid scheduleId)
    {
        // Get all attendance records for this student and schedule
        var attendanceRecords = await _attendanceRepository.GetByStudentAndScheduleAsync(studentId, scheduleId);
        
        // Return the next class number (count of existing + 1)
        return attendanceRecords.Count() + 1;
    }
}
