using Xunit;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;

namespace StudioScheduler.UnitTests.Core.Models;

public class PassDomainTests
{
    [Fact]
    public void GetWeekStart_ReturnsCorrectMondayForDifferentDays()
    {
        // Test that GetWeekStart always returns Monday for any day of the week
        var tuesday = new DateTime(2025, 6, 17); // Tuesday
        var wednesday = new DateTime(2025, 6, 18); // Wednesday  
        var sunday = new DateTime(2025, 6, 22); // Sunday
        var monday = new DateTime(2025, 6, 16); // Monday
        
        var pass = CreateTestPass(PassType.Monthly2Courses);
        
        // All should return the same Monday
        var expectedMonday = new DateTime(2025, 6, 16);
        
        // Use reflection to access private method for testing
        var method = typeof(Pass).GetMethod("GetWeekStart", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        Assert.Equal(expectedMonday, method?.Invoke(null, new object[] { tuesday }));
        Assert.Equal(expectedMonday, method?.Invoke(null, new object[] { wednesday }));
        Assert.Equal(expectedMonday, method?.Invoke(null, new object[] { sunday }));
        Assert.Equal(expectedMonday, method?.Invoke(null, new object[] { monday }));
    }

    [Fact]
    public void SingleClass_CalculateRemainingClasses_ReturnsCorrectValue()
    {
        var pass = CreateTestPass(PassType.SingleClass, totalClasses: 1);
        var currentDate = DateTime.Today;
        
        // No attendances - should have 1 remaining
        var remaining = pass.CalculateRemainingClasses(new List<Attendance>(), currentDate);
        Assert.Equal(1, remaining);
        
        // With attendance - should have 0 remaining
        var attendances = new List<Attendance>
        {
            CreateAttendance(pass.Id, currentDate, wasPresent: true)
        };
        
        remaining = pass.CalculateRemainingClasses(attendances, currentDate);
        Assert.Equal(0, remaining);
    }

    [Fact]
    public void FullPass_CalculateRemainingClasses_ReturnsMaxValue()
    {
        var pass = CreateTestPass(PassType.FullPass);
        var currentDate = DateTime.Today;
        
        var remaining = pass.CalculateRemainingClasses(new List<Attendance>(), currentDate);
        Assert.Equal(int.MaxValue, remaining);
    }

    [Fact]
    public void FlexiPass_CalculateRemainingClasses_RespectsWeeklyLimits()
    {
        var startDate = new DateTime(2025, 6, 16); // Monday
        var endDate = startDate.AddDays(27); // 28-day pass
        var pass = CreateTestPass(PassType.Flexi4Classes, startDate, endDate, totalClasses: 4);
        
        var mondayWeek1 = startDate;
        var tuesdayWeek1 = startDate.AddDays(1);
        var mondayWeek2 = startDate.AddDays(7);
        
        // Week 1: No classes attended - should have 4 remaining (1 per week * 4 weeks)
        var remaining = pass.CalculateRemainingClasses(new List<Attendance>(), mondayWeek1);
        Assert.Equal(4, remaining);
        
        // Week 1: 1 class attended on Monday - should have 3 remaining
        var attendances = new List<Attendance>
        {
            CreateAttendance(pass.Id, mondayWeek1, wasPresent: true)
        };
        
        remaining = pass.CalculateRemainingClasses(attendances, tuesdayWeek1);
        Assert.Equal(3, remaining);
        
        // Week 2: Same attendance - should have 3 remaining (can attend 1 more this week)
        remaining = pass.CalculateRemainingClasses(attendances, mondayWeek2);
        Assert.Equal(3, remaining);
    }

    [Fact]
    public void Flexi8Classes_AllowsTwoClassesPerWeek()
    {
        var startDate = new DateTime(2025, 6, 16); // Monday
        var endDate = startDate.AddDays(27); // 28-day pass
        var pass = CreateTestPass(PassType.Flexi8Classes, startDate, endDate, totalClasses: 8);
        
        var mondayWeek1 = startDate;
        var wednesdayWeek1 = startDate.AddDays(2);
        var fridayWeek1 = startDate.AddDays(4);
        
        // Can use pass on Monday
        Assert.True(pass.CanUseForClass(mondayWeek1, new List<Attendance>()));
        
        // After Monday class, can still use on Wednesday (2 per week allowed)
        var attendancesAfterMonday = new List<Attendance>
        {
            CreateAttendance(pass.Id, mondayWeek1, wasPresent: true)
        };
        Assert.True(pass.CanUseForClass(wednesdayWeek1, attendancesAfterMonday));
        
        // After Monday and Wednesday, cannot use on Friday (weekly limit reached)
        var attendancesAfterTwo = new List<Attendance>
        {
            CreateAttendance(pass.Id, mondayWeek1, wasPresent: true),
            CreateAttendance(pass.Id, wednesdayWeek1, wasPresent: true)
        };
        Assert.False(pass.CanUseForClass(fridayWeek1, attendancesAfterTwo));
    }

    [Fact]
    public void RegularPass_RespectsClassesPerWeekLimit()
    {
        var startDate = new DateTime(2025, 6, 16); // Monday
        var endDate = startDate.AddDays(27); // 28-day pass
        var pass = CreateTestPass(PassType.Monthly3Courses, startDate, endDate, classesPerWeek: 3, totalClasses: 12);
        
        var mondayWeek1 = startDate;
        var wednesdayWeek1 = startDate.AddDays(2);
        var fridayWeek1 = startDate.AddDays(4);
        var sundayWeek1 = startDate.AddDays(6);
        
        // Can use 3 classes in the first week
        Assert.True(pass.CanUseForClass(mondayWeek1, new List<Attendance>()));
        
        var attendancesOne = new List<Attendance>
        {
            CreateAttendance(pass.Id, mondayWeek1, wasPresent: true)
        };
        Assert.True(pass.CanUseForClass(wednesdayWeek1, attendancesOne));
        
        var attendancesTwo = new List<Attendance>
        {
            CreateAttendance(pass.Id, mondayWeek1, wasPresent: true),
            CreateAttendance(pass.Id, wednesdayWeek1, wasPresent: true)
        };
        Assert.True(pass.CanUseForClass(fridayWeek1, attendancesTwo));
        
        // Cannot use 4th class in same week
        var attendancesThree = new List<Attendance>
        {
            CreateAttendance(pass.Id, mondayWeek1, wasPresent: true),
            CreateAttendance(pass.Id, wednesdayWeek1, wasPresent: true),
            CreateAttendance(pass.Id, fridayWeek1, wasPresent: true)
        };
        Assert.False(pass.CanUseForClass(sundayWeek1, attendancesThree));
    }

    [Fact]
    public void CalculateRemainingClasses_ExampleScenario_Monday16June_EndDate26June()
    {
        // Your original example: Today is Monday 16.06.2025, EndDate is 26.06.2025
        var today = new DateTime(2025, 6, 16); // Monday
        var endDate = new DateTime(2025, 6, 26); // Next Thursday
        var startDate = endDate.AddDays(-27); // 28-day pass
        
        var pass = CreateTestPass(PassType.Monthly2Courses, startDate, endDate, classesPerWeek: 2, totalClasses: 8);
        
        // No classes used yet
        var remaining = pass.CalculateRemainingClasses(new List<Attendance>(), today);
        
        // Should be able to attend:
        // - This week (16-22 June): 2 classes possible
        // - Next week (23-26 June, until Thursday): limited by pass expiry
        // Pass expires on 26.06 (Thursday), so partial week
        // Expected: 2 (this week) + some from next week, but limited by total unused classes
        
        Assert.True(remaining > 0);
        Assert.True(remaining <= 8); // Cannot exceed total classes
    }

    [Fact]
    public void GetPassStatus_ReturnsCorrectStatus()
    {
        var currentDate = DateTime.Today;
        var pass = CreateTestPass(PassType.Monthly2Courses, currentDate, currentDate.AddDays(27));
        
        // Active pass with no attendances
        var status = pass.GetPassStatus(new List<Attendance>(), currentDate);
        Assert.Equal(PassStatus.Active, status);
        
        // Expired pass
        var expiredPass = CreateTestPass(PassType.Monthly2Courses, currentDate.AddDays(-30), currentDate.AddDays(-3));
        status = expiredPass.GetPassStatus(new List<Attendance>(), currentDate);
        Assert.Equal(PassStatus.Expired, status);
        
        // Inactive pass
        pass.IsActive = false;
        status = pass.GetPassStatus(new List<Attendance>(), currentDate);
        Assert.Equal(PassStatus.Inactive, status);
    }

    [Fact]
    public void AllowsMakeUpClasses_ReturnsCorrectValue()
    {
        var singleClassPass = CreateTestPass(PassType.SingleClass);
        var regularPass = CreateTestPass(PassType.Monthly2Courses);
        var flexiPass = CreateTestPass(PassType.Flexi4Classes);
        var fullPass = CreateTestPass(PassType.FullPass);
        
        Assert.False(singleClassPass.AllowsMakeUpClasses());
        Assert.True(regularPass.AllowsMakeUpClasses());
        Assert.True(flexiPass.AllowsMakeUpClasses());
        Assert.True(fullPass.AllowsMakeUpClasses());
    }

    [Fact]
    public void GetCompleteWeeksRemaining_CalculatesCorrectly()
    {
        var startDate = new DateTime(2025, 6, 16); // Monday
        var endDate = startDate.AddDays(27); // 28 days = 4 complete weeks
        var pass = CreateTestPass(PassType.Monthly2Courses, startDate, endDate);
        
        // On start date - should have 4 complete weeks
        var weeksRemaining = pass.GetCompleteWeeksRemaining(startDate);
        Assert.Equal(4, weeksRemaining);
        
        // After 1 week
        weeksRemaining = pass.GetCompleteWeeksRemaining(startDate.AddDays(7));
        Assert.Equal(3, weeksRemaining);
        
        // After 3.5 weeks
        weeksRemaining = pass.GetCompleteWeeksRemaining(startDate.AddDays(24));
        Assert.Equal(0, weeksRemaining);
    }

    // Helper methods
    private static Pass CreateTestPass(
        PassType type, 
        DateTime? startDate = null, 
        DateTime? endDate = null,
        int classesPerWeek = 2,
        int totalClasses = 8)
    {
        var start = startDate ?? DateTime.Today;
        var end = endDate ?? start.AddDays(27);
        
        return new Pass
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            StartDate = start,
            EndDate = end,
            Type = type,
            ClassesPerWeek = classesPerWeek,
            TotalClasses = totalClasses,
            IsActive = true
        };
    }
    
    private static Attendance CreateAttendance(Guid passId, DateTime classDate, bool wasPresent = true)
    {
        return new Attendance
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            ScheduleId = Guid.NewGuid(),
            ClassDate = classDate,
            WasPresent = wasPresent,
            PassUsed = passId
        };
    }
}
