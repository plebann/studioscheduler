# StudioScheduler Playwright Tests

This project contains API tests for the StudioScheduler application using Playwright for .NET.

## Overview

We have successfully implemented a foundation for Playwright API testing with the following structure:

- **BaseApiTest.cs**: Base class for all API tests with shared setup/teardown
- **SchedulesApiTests.cs**: Comprehensive tests for `/api/schedules/weekly` endpoint
- **AttendanceApiTests.cs**: Comprehensive tests for `/api/attendance/class/{id}` endpoint

## Current Status

✅ **Project Setup Complete**
- Playwright NUnit package configured
- Base test infrastructure created
- Test data configuration ready
- WebApplicationFactory integration attempted

⚠️ **Known Issue**
- Missing `Microsoft.Playwright.deps.json` file causing WebApplicationFactory integration issues
- Tests are structured correctly but need final configuration fixes

## Test Coverage

### SchedulesApiTests (7 tests)
1. `GetWeeklySchedule_ShouldReturnSuccessWithValidStructure` - Validates response structure
2. `GetWeeklySchedule_ShouldReturnCorrectScheduleSlotStructure` - Validates ScheduleSlotDto
3. `GetWeeklySchedule_ShouldReturnCorrectDanceStyles` - Validates expected dance styles
4. `GetWeeklySchedule_ShouldReturnCorrectColorMapping` - Validates color coding by dance style
5. `GetWeeklySchedule_ShouldReturnClassesInTimeOrder` - Validates time ordering
6. `GetWeeklySchedule_ShouldHaveCorrectResponseHeaders` - Validates HTTP headers
7. `GetWeeklySchedule_ShouldReturnValidJson` - Validates JSON format

### AttendanceApiTests (12 tests)
1. `GetClassSchedule_WithValidId_ShouldReturnSuccessWithValidStructure` - Basic structure validation
2. `GetClassSchedule_WithValidId_ShouldReturnCorrectScheduleId` - ID validation
3. `GetClassSchedule_WithValidId_ShouldReturnValidStartTime` - Time validation
4. `GetClassSchedule_WithValidId_ShouldReturnValidDayOfWeek` - Day validation
5. `GetClassSchedule_WithValidId_ShouldReturnValidEnrolledStudents` - Student data validation
6. `GetClassSchedule_WithValidId_ShouldReturnCorrectDanceStyleAndLevel` - Dance info validation
7. `GetClassSchedule_WithInvalidGuid_ShouldReturnBadRequest` - Error handling
8. `GetClassSchedule_WithNonExistentId_ShouldReturnNotFound` - Error handling
9. `GetClassSchedule_WithValidId_ShouldHaveCorrectResponseHeaders` - HTTP headers
10. `GetClassSchedule_WithValidId_ShouldReturnValidJson` - JSON validation
11. `GetClassSchedule_WithValidId_ShouldReturnConsistentData` - Data consistency
12. `GetClassSchedule_ResponseTime_ShouldBeReasonable` - Performance testing

## Next Steps

To complete the Playwright setup:

1. **Fix WebApplicationFactory Integration**: Resolve the Microsoft.Playwright.deps.json issue
2. **Install Playwright Browsers**: Run browser installation after fixing the integration
3. **Run Tests**: Execute the comprehensive test suite
4. **Expand Coverage**: Add tests for other controllers (Classes, Locations, Rooms)

## Alternative Approaches

If WebApplicationFactory integration proves complex, consider:

1. **Direct HTTP Client**: Use HttpClient directly against running server
2. **API-Only Testing**: Focus on Playwright's API testing capabilities without browser integration
3. **Hybrid Approach**: Keep integration tests as-is, use Playwright for E2E UI testing

## Benefits Achieved

Even with the current setup incomplete, we have:

✅ **Comprehensive Test Structure**: Well-organized, maintainable test code
✅ **Real API Validation**: Tests that validate actual API contracts
✅ **Error Scenario Coverage**: Proper error handling validation
✅ **Performance Testing**: Response time validation
✅ **Data Validation**: Comprehensive response structure validation
✅ **Future-Ready**: Foundation for expanding to full E2E testing

## Integration with Existing Tests

This Playwright setup complements your existing testing:
- **Unit Tests**: Fast, isolated component testing
- **Integration Tests**: Database and service layer testing
- **Playwright Tests**: API contract and E2E testing
- **Manual HTTP Tests**: Quick development testing

The hybrid approach provides comprehensive test coverage across all application layers.
