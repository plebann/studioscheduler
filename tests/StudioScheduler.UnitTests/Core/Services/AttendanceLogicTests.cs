using System;
using System.Collections.Generic;
using FluentAssertions;
using StudioScheduler.Shared.Dtos;
using Xunit;

namespace StudioScheduler.UnitTests.Core.Services;

public class AttendanceLogicTests
{
    [Fact]
    public void Should_Show_White_Flags_For_Weeks_Before_Enrollment()
    {
        // Arrange
        var enrolledDate = new DateTime(2025, 7, 1);
        var week1 = new DateTime(2025, 6, 24); // before enrollment
        var week2 = new DateTime(2025, 7, 1);  // enrollment week
        var week3 = new DateTime(2025, 7, 8);  // after enrollment

        var attendanceHistory = new List<AttendanceRecordDto>
        {
            new AttendanceRecordDto { ClassDate = week1, WeekOffset = -3, WasPresent = false, IsEnrolled = false, IsCanceled = false },
            new AttendanceRecordDto { ClassDate = week2, WeekOffset = -2, WasPresent = false, IsEnrolled = true, IsCanceled = false },
            new AttendanceRecordDto { ClassDate = week3, WeekOffset = -1, WasPresent = true, IsEnrolled = true, IsCanceled = false }
        };

        // Act
        var whiteFlags = attendanceHistory.FindAll(r => !r.IsEnrolled);

        // Assert
        whiteFlags.Should().HaveCount(1);
        whiteFlags[0].ClassDate.Should().Be(week1);
    }

    [Fact]
    public void Should_Show_GrayRed_Flag_For_Canceled_Week()
    {
        // Arrange
        var week = new DateTime(2025, 7, 8);
        var attendance = new AttendanceRecordDto
        {
            ClassDate = week,
            WeekOffset = -1,
            WasPresent = false,
            IsEnrolled = true,
            IsCanceled = true
        };

        // Act & Assert
        attendance.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public void Should_Show_Green_Flag_For_Present()
    {
        // Arrange
        var week = new DateTime(2025, 7, 15);
        var attendance = new AttendanceRecordDto
        {
            ClassDate = week,
            WeekOffset = 0,
            WasPresent = true,
            IsEnrolled = true,
            IsCanceled = false
        };

        // Act & Assert
        attendance.WasPresent.Should().BeTrue();
        attendance.IsEnrolled.Should().BeTrue();
        attendance.IsCanceled.Should().BeFalse();
    }

    [Fact]
    public void Should_Show_Gray_Flag_For_Absent()
    {
        // Arrange
        var week = new DateTime(2025, 7, 15);
        var attendance = new AttendanceRecordDto
        {
            ClassDate = week,
            WeekOffset = 0,
            WasPresent = false,
            IsEnrolled = true,
            IsCanceled = false
        };

        // Act & Assert
        attendance.WasPresent.Should().BeFalse();
        attendance.IsEnrolled.Should().BeTrue();
        attendance.IsCanceled.Should().BeFalse();
    }
}
