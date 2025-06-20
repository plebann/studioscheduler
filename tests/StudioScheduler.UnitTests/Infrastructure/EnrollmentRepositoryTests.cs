using Microsoft.EntityFrameworkCore;
using StudioScheduler.Core.Models;
using StudioScheduler.Infrastructure.Data;
using StudioScheduler.Infrastructure.Repositories;
using Xunit;

namespace StudioScheduler.UnitTests.Infrastructure;

public class EnrollmentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EnrollmentRepository _repository;

    public EnrollmentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EnrollmentRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_WithNoExistingEnrollment_ShouldCreateSuccessfully()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateAsync(enrollment);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(enrollment.Id, result.Id);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task CreateAsync_WithExistingActiveEnrollment_ShouldThrowDuplicateException()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        
        var existingEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow.AddDays(-1),
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        
        // Add directly to context
        _context.Enrollments.Add(existingEnrollment);
        await _context.SaveChangesAsync();

        var newEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EnrollmentRepository.DuplicateEnrollmentException>(
            () => _repository.CreateAsync(newEnrollment));
        
        Assert.Equal(studentId, exception.StudentId);
        Assert.Equal(scheduleId, exception.ScheduleId);
    }

    [Fact]
    public async Task GetActiveByStudentAndScheduleAsync_WithActiveEnrollment_ShouldReturnEnrollment()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        // Add directly to context (no navigation properties needed for this test)
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveByStudentAndScheduleAsync(studentId, scheduleId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(enrollment.Id, result.Id);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetActiveByStudentAndScheduleAsync_WithInactiveEnrollment_ShouldReturnNull()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = false // Inactive
        };
        
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveByStudentAndScheduleAsync(studentId, scheduleId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrReactivateAsync_WithExistingInactiveEnrollment_ShouldReactivate()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var existingEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow.AddDays(-7),
            IsActive = false // Inactive
        };
        
        await _context.Enrollments.AddAsync(existingEnrollment);
        await _context.SaveChangesAsync();

        var newEnrollmentData = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateOrReactivateAsync(newEnrollmentData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingEnrollment.Id, result.Id); // Should return existing enrollment
        Assert.True(result.IsActive); // Should be reactivated
        Assert.Equal(newEnrollmentData.EnrolledDate.Date, result.EnrolledDate.Date); // Should update date
    }

    [Fact]
    public async Task CreateOrReactivateAsync_WithNoExistingEnrollment_ShouldCreateNew()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateOrReactivateAsync(enrollment);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(enrollment.Id, result.Id);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task HasActiveEnrollmentAsync_WithActiveEnrollment_ShouldReturnTrue()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = true
        };
        
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasActiveEnrollmentAsync(studentId, scheduleId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasActiveEnrollmentAsync_WithNoActiveEnrollment_ShouldReturnFalse()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();

        // Act
        var result = await _repository.HasActiveEnrollmentAsync(studentId, scheduleId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetDistinctStudentCountAsync_WithDuplicateEnrollments_ShouldReturnDistinctCount()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var student1Id = Guid.NewGuid();
        var student2Id = Guid.NewGuid();

        // Create multiple enrollments for same students (shouldn't happen with new logic, but test defensive code)
        var enrollments = new[]
        {
            new Enrollment { Id = Guid.NewGuid(), StudentId = student1Id, ScheduleId = scheduleId, IsActive = true, EnrolledDate = DateTime.UtcNow },
            new Enrollment { Id = Guid.NewGuid(), StudentId = student2Id, ScheduleId = scheduleId, IsActive = true, EnrolledDate = DateTime.UtcNow },
            new Enrollment { Id = Guid.NewGuid(), StudentId = student1Id, ScheduleId = scheduleId, IsActive = false, EnrolledDate = DateTime.UtcNow } // Inactive, shouldn't count
        };
        
        await _context.Enrollments.AddRangeAsync(enrollments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDistinctStudentCountAsync(scheduleId);

        // Assert
        Assert.Equal(2, result); // Should count distinct active students only
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
