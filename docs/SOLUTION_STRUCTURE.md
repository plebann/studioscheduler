# StudioScheduler Solution Structure and Architecture

## Overview
StudioScheduler is a cloud-hosted web application for managing dance/fitness studio classes, supporting both admin/operator (desktop) and student (mobile) interfaces. The solution is modular, scalable, and designed for Azure deployment.

**Based on SalsaMe Dance Studio** - A real-world dance studio in Lublin, Poland, with 32+ weekly classes across multiple dance styles and a comprehensive pass system.

---

## Solution Structure
```
StudioScheduler/
├── src/
│   ├── StudioScheduler.Client/         # Blazor WebAssembly frontend (MudBlazor, Fluxor, PWA)
│   ├── StudioScheduler.Server/         # ASP.NET Core API backend (SignalR, JWT, EF Core)
│   ├── StudioScheduler.Core/           # Domain models, interfaces, business logic
│   ├── StudioScheduler.Infrastructure/ # Data access, repositories, external services
│   └── StudioScheduler.Shared/         # DTOs and models shared between client/server
├── tests/
│   ├── StudioScheduler.UnitTests/      # Unit tests
│   ├── StudioScheduler.IntegrationTests/ # Integration tests
│   └── StudioScheduler.ReservationTests/ # Reservation-related tests (additional)
```

---

## Project Details

### StudioScheduler.Client
- **Type:** Blazor WebAssembly (.NET 9, upgrade to .NET 10 LTS planned)
- **UI:** MudBlazor
- **State:** Fluxor
- **Features:** PWA, authentication UI, layouts, navigation, core components

### StudioScheduler.Server
- **Type:** ASP.NET Core 9 API
- **Features:**
  - Entity Framework Core 9
  - SignalR for real-time updates
  - JWT-based authentication (role-based)
  - Controllers for classes, locations, rooms, schedules
  - Mock repositories for development/testing
  - Registered services: DanceClassService, LocationService, ScheduleService
  - API endpoints for CRUD operations

### StudioScheduler.Core
- **Purpose:** Domain models, interfaces, business logic
- **Key Entities:** User, Class, Schedule, Pass, Reservation, Location, Room
- **Docs:** `docs/schedule_implementation_plan.md` (data model, implementation phases)

### StudioScheduler.Infrastructure
- **Purpose:** Data access, repository implementations, external service integration
- **Database:** Entity Framework Core with SQLite (development) / PostgreSQL (production)
- **Database File:** `src/StudioScheduler.Server/studioscheduler.db`
- **Repositories:** DanceClassRepository, LocationRepository, RoomRepository, ScheduleRepository, AttendanceRepository, EnrollmentRepository, StudentRepository
- **Data Seeding:** DataSeedingService loads initial data from JSON files into database during first startup

### StudioScheduler.Shared
- **Purpose:** DTOs and models shared between client and server

### tests/
- **UnitTests:** Core logic/unit tests
- **IntegrationTests:** End-to-end and API integration tests
- **ReservationTests:** Reservation-specific tests (additional)

---

## Differences from Documentation
- The `tools/` folder (for DB migrations/scripts) is not present in the current solution.
- There is an additional test project: `StudioScheduler.ReservationTests`.

---

## Data Model (from `schedule_implementation_plan.md`)
- **Location**: id, name, address, description, isActive, capacity, opening/closing times
- **Room**: id, locationId, name, capacity, description, equipment
- **DanceClass**: id, name, description, style, isActive - Dance style definitions (Bachata, Salsa, etc.)
- **Schedule**: id, name, locationId, danceClassId, level, instructorId, roomId, startTime, duration, isRecurring, isActive - Actual class groups
- **Relationships:**
  - Location 1--* Room
  - Location 1--* Schedule
  - Schedule *--1 DanceClass (each class group belongs to a dance style)
  - Room 1--* Schedule (each class group is held in a room)

### Architecture Overview
The system models real-world dance studio operations:

#### DanceClass (Dance Style)
- **Static definitions** of dance styles: "Bachata", "Salsa Cubana", "Salsa on1"
- **Style categorization**: BACHATA, SALSA, ZOUK, KIZOMBA, etc.
- **Reusable templates** for creating class groups

#### Schedule (Class Group)
- **Dynamic class groups** with specific characteristics
- **Level progression**: P1 → P2 → P3 → S1 → S2 → S3
- **Instructor assignment**: Can change over time
- **Time and location**: Specific weekly schedule
- **Group evolution**: Groups can merge, split, or change characteristics

#### Real-World Business Scenarios
1. **New Semester Planning**: Create multiple Schedules for popular dance styles
2. **Group Progression**: Update Schedule.Level as students advance
3. **Instructor Substitution**: Change Schedule.InstructorId temporarily
4. **Group Consolidation**: Merge small groups during reorganization
5. **Schedule Changes**: Update time/location for existing groups

---

## Business Model Analysis

### Real-World Pass System (SalsaMe Studio)
Based on analysis of https://salsame.pl/en/price-list/, our system models a real dance studio with:

#### Pass Types & Pricing (PLN)
- **Monthly Passes (28 days validity):**
  - 1 course/week (4 classes) - 130 PLN
  - 2 courses/week (8 classes) - 200 PLN
  - 3 courses/week (12 classes) - 240 PLN
  - 4 courses/week (16 classes) - 280 PLN
  - 5 courses/week (20 classes) - 320 PLN

- **FLEXI Passes (28 days validity):**
  - 4 different classes - 140 PLN
  - 8 different classes - 220 PLN

- **Other Options:**
  - Single class (P2+ only) - 40 PLN
  - FULLPASS (unlimited) - 350 PLN
  - Private lessons - 180-200 PLN/hour

#### Business Rules
- **28-day validity** (not calendar months)
- **Make-up classes** allowed within validity period
- **Pass freezing** available once per 6 months (2-week max, additional cost)
- **Level restrictions** for single classes (no P1/beginners)
- **No extensions** - passes expire regardless of usage

### Current Implementation Status
- ✅ **Basic pass model** with core fields
- ✅ **PassType enum** (needs expansion for SalsaMe types)
- ✅ **Enhanced Schedule Model** with DayOfWeek + TimeSpan for weekly recurring patterns (2025-06-19)
- ✅ **Comprehensive Test Coverage** - All 112 tests passing across all test projects
- ❌ **Pricing system** not implemented
- ❌ **28-day validity calculation** not implemented
- ❌ **Make-up class tracking** not implemented
- ❌ **Pass freezing system** not implemented
- ❌ **Level restrictions** not implemented

**See `SALSAME_BUSINESS_ANALYSIS.md` for detailed comparison and implementation recommendations.**

---

## Implementation Phases
1. **Project Setup & Core Infrastructure**: Solution structure, project setup, EF Core, authentication
2. **Backend Implementation**: API endpoints, business logic, service layer, error handling, logging
3. **Frontend Foundation**: Blazor setup, authentication UI, layouts, navigation
4. **Feature Implementation**: Class/user/pass/reservation management, real-time updates
5. **Azure Infrastructure**: App Service, PostgreSQL, Application Insights, CI/CD

---

## API Testing & Mock Data
- **HTTP Test Files:** Located in `src/StudioScheduler.Server/Controllers/Tests/`
  - `AllEndpoints.http`: Quick comprehensive test suite
  - `ClassesController.http`, `LocationsController.http`: Detailed CRUD and error tests
  - `SchedulesController.http`: Weekly schedule endpoint tests
  - `README.md`, `SETUP_SUMMARY.md`: Testing instructions, troubleshooting, architecture
- **Mock Data:**
  - 27+ dance classes (SalsaMe Studio) with proper Style and Level fields
  - 1 location (SalsaMe Dance Studio)
  - 2 rooms (Studio A, Studio B)
  - 32 schedule entries (real SalsaMe schedule data from 2025)
- **Architecture:**
  - API Request → Controller → Service → Repository → SQLite Database (Entity Framework)

---

## Schedule System Implementation (Updated 2025-06-15)

### Schedule Page Data Flow
- **Client:** The Schedule page (`/schedule`) fetches data from `api/schedules/weekly`
- **Backend:** `SchedulesController.GetWeeklySchedule()` method processes the request:
  1. Loads all active schedules from SQLite database via Entity Framework
  2. **Loads dance class data** from `DanceClassService` to get accurate style, level, and description
  3. Groups schedules by day of week (Monday-Sunday)
  4. Creates `ScheduleSlotDto` objects with proper formatting

### Schedule Data Integration
- **Fixed Issue:** Controller now uses actual DanceClass data instead of parsing schedule names
- **Color Mapping:** Based on DanceClass.Style field:
  - `STYLE` (Ladies Styling, High Heels) → `#E40046` (Pink/Red)
  - `CUBANA` (Salsa Kubańska) → `#B08A47` (Brown/Gold)
  - `SALSA` (Salsa on1/on2) → `#333333` (Dark Gray)
  - `BACHATA` → `#166693` (Blue)
  - `RUEDA` → `#DFAF29` (Yellow/Gold)
  - `ZOUK` → `#6A1B9A` (Purple)
  - `KIZOMBA` → `#007C5A` (Green)
- **Level Formatting:** Properly displays "Level P1", "Level S3", "OPEN level", etc.
- **Style Names:** Correctly shows "SALSA KUBAŃSKA", "HIGH HEELS SEXY DANCE", "KIZOMBA i SEMBA"

### Current Schedule (SalsaMe Dance Studio 2025)
- **Monday-Thursday:** 6 classes each (16:45-22:15)
- **Friday & Sunday:** 4 classes each (17:40-21:20)
- **Saturday:** No classes
- **Total:** 32 classes per week
- **All classes:** 55 minutes duration

### Technical Implementation
- **SchedulesController:** Injects both `IScheduleService` and `IDanceClassService`
- **Data Lookup:** Uses `danceClassDict.TryGetValue(schedule.DanceClassId)` for accurate data
- **Helper Methods:** `GetBackgroundColorByStyle()`, `FormatLevel()`, `FormatDanceStyle()`
- **Responsive UI:** CSS supports both desktop table view and mobile card view

---

## Key Features
- Multi-location and room support
- Schedule management and filtering
- Student registration and attendance tracking
- Real-time updates (SignalR)
- Role-based authentication
- Azure-ready deployment
- **Real-world business model** based on operational dance studio

---

## Documentation Files

### Business & Analysis Documentation
- **`SALSAME_BUSINESS_ANALYSIS.md`** - Detailed business model analysis and pass system comparison

### API Testing Documentation
- **`src/StudioScheduler.Server/Controllers/Tests/README.md`** - API testing instructions and setup
- **`src/StudioScheduler.Server/Controllers/Tests/SETUP_SUMMARY.md`** - Testing troubleshooting and architecture notes

### HTTP Test Files
- **`src/StudioScheduler.Server/Controllers/Tests/AllEndpoints.http`** - Comprehensive API test suite
- **`src/StudioScheduler.Server/Controllers/Tests/ClassesController.http`** - Classes endpoint tests
- **`src/StudioScheduler.Server/Controllers/Tests/LocationsController.http`** - Locations endpoint tests
- **`src/StudioScheduler.Server/Controllers/Tests/SchedulesController.http`** - Schedule endpoint tests

---

## Troubleshooting & Next Steps
- Build and run the server before testing
- Use REST Client extension for API tests
- Add more controllers/tests as needed
- Integration and Swagger/OpenAPI support available
