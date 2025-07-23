# Task List: Remove Unused Controller Methods (Safe Refactor)

## AttendanceController (src/StudioScheduler.Server/Controllers/AttendanceController.cs)
- [ ] GetClassSchedule
  - Referenced in: src/StudioScheduler.Server/Controllers/AttendanceController.cs (method implementation)
  - API usage: Found in endpoint search (api/attendance/class) [external usage possible]
- [ ] MarkAttendance
  - Referenced in: src/StudioScheduler.Server/Controllers/AttendanceController.cs (method implementation)
  - API usage: Found in endpoint search (api/attendance/mark) [external usage possible]
- [ ] SearchStudents
  - Referenced in: src/StudioScheduler.Server/Controllers/AttendanceController.cs (method implementation)
  - API usage: Found in endpoint search (api/attendance/search) [external usage possible]

## ClassesController (src/StudioScheduler.Server/Controllers/ClassesController.cs)
- [ ] GetClasses
  - Referenced in: src/StudioScheduler.Server/Controllers/ClassesController.cs (method implementation)
  - API usage: Found in endpoint search (api/classes) [external usage possible]
- [ ] GetClass
  - Referenced in: src/StudioScheduler.Server/Controllers/ClassesController.cs (method implementation)
  - API usage: Found in endpoint search (api/classes/{id}) [external usage possible]
- [ ] CreateClass
  - Referenced in: src/StudioScheduler.Server/Controllers/ClassesController.cs (method implementation)
  - API usage: Found in endpoint search (api/classes) [external usage possible]
- [ ] UpdateClass
  - Referenced in: src/StudioScheduler.Server/Controllers/ClassesController.cs (method implementation)
  - API usage: Found in endpoint search (api/classes/{id}) [external usage possible]
- [ ] DeleteClass
  - Referenced in: src/StudioScheduler.Server/Controllers/ClassesController.cs (method implementation)
  - API usage: Found in endpoint search (api/classes/{id}) [external usage possible]

## LocationsController (src/StudioScheduler.Server/Controllers/LocationsController.cs)
- [ ] GetLocations
  - Referenced in: src/StudioScheduler.Server/Controllers/LocationsController.cs (method implementation)
  - API usage: Found in endpoint search (api/locations) [external usage possible]
- [ ] GetLocation
  - Referenced in: src/StudioScheduler.Server/Controllers/LocationsController.cs (method implementation)
  - API usage: Found in endpoint search (api/locations/{id}) [external usage possible]
- [ ] CreateLocation
  - Referenced in: src/StudioScheduler.Server/Controllers/LocationsController.cs (method implementation)
  - API usage: Found in endpoint search (api/locations) [external usage possible]
- [ ] UpdateLocation
  - Referenced in: src/StudioScheduler.Server/Controllers/LocationsController.cs (method implementation)
  - API usage: Found in endpoint search (api/locations/{id}) [external usage possible]
- [ ] DeleteLocation
  - Referenced in: src/StudioScheduler.Server/Controllers/LocationsController.cs (method implementation)
  - API usage: Found in endpoint search (api/locations/{id}) [external usage possible]
- [ ] GetLocationRooms
  - Referenced in: src/StudioScheduler.Server/Controllers/LocationsController.cs (method implementation)
  - API usage: Found in endpoint search (api/locations/{id}/rooms) [external usage possible]
- [ ] GetLocationSchedules
  - Referenced in: src/StudioScheduler.Server/Controllers/LocationsController.cs (method implementation)
  - API usage: Found in endpoint search (api/locations/{id}/schedules) [external usage possible]

## PassController (src/StudioScheduler.Server/Controllers/PassController.cs)
- [ ] GetAllPasses
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint GET api/pass (no direct client usage found)
- [ ] GetPassById
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint GET api/pass/{id} (no direct client usage found)
- [ ] GetPassesByUserId
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint GET api/pass/user/{userId} (no direct client usage found)
- [ ] GetCurrentActivePassForUser
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Referenced by StudioScheduler.Client.Services.PassService.GetCurrentActivePassAsync (src/StudioScheduler.Client/Services/PassService.cs)
- [ ] GetActivePasses
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint GET api/pass/active (no direct client usage found)
- [ ] GetPassesExpiringInDays
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint GET api/pass/expiring (no direct client usage found)
- [ ] CreatePass
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint POST api/pass (no direct client usage found)
- [ ] UpdatePass
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint PUT api/pass/{id} (no direct client usage found)
- [ ] DeletePass
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint DELETE api/pass/{id} (no direct client usage found)
- [ ] GetAvailablePassTypes
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint GET api/pass/types (no direct client usage found)
- [ ] PurchasePass
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Referenced by StudioScheduler.Client.Services.PassService.PurchasePassAsync (src/StudioScheduler.Client/Services/PassService.cs)
- [ ] GetAvailableMonthlyPassTypes
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Referenced by StudioScheduler.Client.Services.PassService.GetMonthlyPassTypesAsync (src/StudioScheduler.Client/Services/PassService.cs)
- [ ] GetPassUsageStats
  - Referenced in: src/StudioScheduler.Server/Controllers/PassController.cs (method implementation)
  - API usage: Endpoint GET api/pass/{id}/stats (no direct client usage found)

## RoomsController (src/StudioScheduler.Server/Controllers/RoomsController.cs)
- [ ] GetRooms
  - Referenced in: src/StudioScheduler.Server/Controllers/RoomsController.cs (method implementation)
  - API usage: Found in endpoint search (api/rooms) [external usage possible]
- [ ] GetRoom
  - Referenced in: src/StudioScheduler.Server/Controllers/RoomsController.cs (method implementation)
  - API usage: Found in endpoint search (api/rooms/{id}) [external usage possible]
- [ ] CreateRoom
  - Referenced in: src/StudioScheduler.Server/Controllers/RoomsController.cs (method implementation)
  - API usage: Found in endpoint search (api/rooms) [external usage possible]
- [ ] UpdateRoom
  - Referenced in: src/StudioScheduler.Server/Controllers/RoomsController.cs (method implementation)
  - API usage: Found in endpoint search (api/rooms/{id}) [external usage possible]
- [ ] DeleteRoom
  - Referenced in: src/StudioScheduler.Server/Controllers/RoomsController.cs (method implementation)
  - API usage: Found in endpoint search (api/rooms/{id}) [external usage possible]

## SchedulesController (src/StudioScheduler.Server/Controllers/SchedulesController.cs)
- [ ] GetSchedules
  - Referenced in: src/StudioScheduler.Server/Controllers/SchedulesController.cs (method implementation)
  - API usage: Found in endpoint search (api/schedules) [external usage possible]
- [ ] GetSchedule
  - Referenced in: src/StudioScheduler.Server/Controllers/SchedulesController.cs (method implementation)
  - API usage: Found in endpoint search (api/schedules/{id}) [external usage possible]
- [ ] CreateSchedule
  - Referenced in: src/StudioScheduler.Server/Controllers/SchedulesController.cs (method implementation)
  - API usage: Found in endpoint search (api/schedules) [external usage possible]
- [ ] UpdateSchedule
  - Referenced in: src/StudioScheduler.Server/Controllers/SchedulesController.cs (method implementation)
  - API usage: Found in endpoint search (api/schedules/{id}) [external usage possible]
- [ ] DeleteSchedule
  - Referenced in: src/StudioScheduler.Server/Controllers/SchedulesController.cs (method implementation)
  - API usage: Found in endpoint search (api/schedules/{id}) [external usage possible]
- [ ] GetWeeklySchedule
  - Referenced in: src/StudioScheduler.Server/Controllers/SchedulesController.cs (method implementation)
  - API usage: Found in endpoint search (api/schedules/weekly) [external usage possible]

## StudentsController (src/StudioScheduler.Server/Controllers/StudentsController.cs)
- [ ] GetStudents
  - Referenced in: src/StudioScheduler.Server/Controllers/StudentsController.cs (method implementation)
  - API usage: Found in endpoint search (api/students) [external usage possible]
- [ ] GetStudent
  - Referenced in: src/StudioScheduler.Server/Controllers/StudentsController.cs (method implementation)
  - API usage: Found in endpoint search (api/students/{id}) [external usage possible]
- [ ] CreateStudent
  - Referenced in: src/StudioScheduler.Server/Controllers/StudentsController.cs (method implementation)
  - API usage: Found in endpoint search (api/students) [external usage possible]
- [ ] UpdateStudent
  - Referenced in: src/StudioScheduler.Server/Controllers/StudentsController.cs (method implementation)
  - API usage: Found in endpoint search (api/students/{id}) [external usage possible]
- [ ] DeleteStudent
  - Referenced in: src/StudioScheduler.Server/Controllers/StudentsController.cs (method implementation)
  - API usage: Found in endpoint search (api/students/{id}) [external usage possible]
- [ ] SearchStudents
  - Referenced in: src/StudioScheduler.Server/Controllers/StudentsController.cs (method implementation)
  - API usage: Found in endpoint search (api/students/search) [external usage possible]

---

**Instructions:**
- For each controller method, search the entire codebase for references (including Blazor components, services, other controllers, and tests).
- List all references for each method, including REST endpoint usage and client/service calls.
- If a method has zero references outside its own implementation and no API usage, mark it as a candidate for removal.
- Do not remove any controller method until approved.
