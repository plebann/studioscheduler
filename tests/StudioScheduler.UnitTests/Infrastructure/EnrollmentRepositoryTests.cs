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
            EnrolledDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(enrollment);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(enrollment.Id, result.Id);
    }

    [Fact]
    public async Task CreateAsync_WithExistingEnrollment_ShouldThrowDuplicateException()
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
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EnrollmentRepository.DuplicateEnrollmentException>(
            () => _repository.CreateAsync(newEnrollment));
        
        Assert.Equal(studentId, exception.StudentId);
        Assert.Equal(scheduleId, exception.ScheduleId);
    }

    [Fact]
    public async Task CreateOrReactivateAsync_WithExistingEnrollment_ShouldUpdateDate()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var existingEnrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow.AddDays(-7)
        };
        
        await _context.Enrollments.AddAsync(existingEnrollment);
        await _context.SaveChangesAsync();

        var newEnrollmentData = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateOrReactivateAsync(newEnrollmentData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingEnrollment.Id, result.Id); // Should return existing enrollment
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
            EnrolledDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateOrReactivateAsync(enrollment);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(enrollment.Id, result.Id);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
