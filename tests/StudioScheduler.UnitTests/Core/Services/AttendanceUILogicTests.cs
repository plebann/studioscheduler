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
            AttendanceHistory = [],
            IsMarkedPresentToday = false,
            CanAttendToday = false
        };

        // Act
        var skipped = anna.AttendanceHistory.FindAll(r => r.IsCanceled && !r.WasPresent);
        var present = anna.AttendanceHistory.FindAll(r => r.WasPresent && !r.IsCanceled);

        // Assert
        skipped.Should().HaveCount(0, "Anna should have 0 skipped/canceled classes");
        present.Should().HaveCount(0, "Anna should have 0 present classes (today)");
    }
}
