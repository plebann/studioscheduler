# Task List: Remove Unused Service Methods and Related Tests

## DanceClassService (src/StudioScheduler.Infrastructure/Services/DanceClassService.cs)
- [x] GetByStyleAsync
- [x] ExistsAsync
- [x] GetClassSchedulesAsync
- [x] IsInstructorAvailableAsync
- [x] GetCurrentEnrollmentAsync

## LocationService (src/StudioScheduler.Infrastructure/Services/LocationService.cs)

## RoomService (src/StudioScheduler.Infrastructure/Services/RoomService.cs)
- [x] ExistsAsync
- [x] IsRoomAvailableAsync
- [x] GetRoomSchedulesAsync
- [x] GetRoomByLocationAndNameAsync

## ScheduleService (src/StudioScheduler.Infrastructure/Services/ScheduleService.cs)
- [x] GetByLocationAsync
- [x] GetByDayOfWeekAsync
- [x] GetByDanceClassAsync
- [x] ExistsAsync
- [x] CancelClassAsync
- [x] IsTimeSlotAvailableAsync
- [x] GetActiveSchedulesAsync
- [x] GetSchedulesByInstructorAsync
- [x] GetAvailableSpotsAsync
- [x] IsScheduleConflictAsync

## Related Test Files
- [x] tests/StudioScheduler.UnitTests/Core/Services/LocationServiceTests.cs
- [x] tests/StudioScheduler.UnitTests/Core/Services/RoomServiceTests.cs

---

**Instructions:**
- Remove all listed methods from their respective service classes and interfaces.
- Remove the related test files if they only cover the listed methods.

---

# Reference Analysis

No references found in the codebase for any listed method (other than their own implementation).
