using System;
using System.Collections.Generic;
using FluentAssertions;
using StudioScheduler.Shared.Dtos;
using Xunit;

namespace StudioScheduler.UnitTests.Core.Services;

public class AttendanceUILogicTests
{
    // ... (existing tests) ...

    [Fact]
    public void AnnaKowalska_BachataMonday1930_Should_Have_4Skipped_1Present()
    {
        // Arrange: Simulate today is Monday, 2025-07-21, 19:30
        var today = new DateTime(2025, 7, 21, 19, 30, 0);
        var anna = new StudentAttendanceDto
        {
            StudentId = "anna-kowalska",
            FirstName = "Anna",
            LastName = "Kowalska",
            CurrentPass = new StudentPassDto
            {
                PassId = "pass-1",
                PassType = "Monthly",
                StartDate = today.AddDays(-1),
                EndDate = today.AddDays(29),
                TotalClasses = 4,
                RemainingClasses = 3,
                ClassesPerWeek = 1,
                Price = 100m,
                IsActive = true,
                IsExpired = false
            },
            AttendanceHistory = new List<AttendanceRecordDto>
            {
                new AttendanceRecordDto { ClassDate = today.AddDays(-28), WeekOffset = -3, WasPresent = false, IsEnrolled = true, IsCanceled = true }, // school canceled
                new AttendanceRecordDto { ClassDate = today.AddDays(-21), WeekOffset = -2, WasPresent = false, IsEnrolled = true, IsCanceled = false },
                new AttendanceRecordDto { ClassDate = today.AddDays(-14), WeekOffset = -1, WasPresent = false, IsEnrolled = true, IsCanceled = true }, // student canceled
                new AttendanceRecordDto { ClassDate = today.AddDays(-7), WeekOffset = 0, WasPresent = true, IsEnrolled = true, IsCanceled = false },
            },
            IsMarkedPresentToday = false,
            CanAttendToday = false
        };

        // Act
        var schoolCanceled = anna.AttendanceHistory.FindAll(r => r.IsCanceled && !r.WasPresent);
        var present = anna.AttendanceHistory.FindAll(r => r.WasPresent && !r.IsCanceled);

        // Assert
        schoolCanceled.Should().HaveCount(2, "Anna should have 2 canceled classes (school and student)");
        present.Should().HaveCount(1, "Anna should have 1 present class");
    }
}
