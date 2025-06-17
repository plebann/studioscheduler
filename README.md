# Studio Scheduler

## Project Overview
Studio Scheduler is a cloud-hosted web application for managing dance/fitness studio classes, with both desktop (admin/operator) and mobile (student) interfaces.

## Technical Stack

### Frontend
- Blazor WebAssembly (.NET 9, upgrade to .NET 10 LTS planned)
- MudBlazor for UI components
- Fluxor for state management
- PWA capabilities for mobile

### Backend
- ASP.NET Core 9 API
- Entity Framework Core 9 with SQLite (development)
- Repository pattern implementation
- SignalR for real-time features (planned)
- JWT-based authentication (planned)

### Infrastructure
- SQLite database (development)
- Azure hosting planned (App Service + PostgreSQL)
- Azure Application Insights (planned)
- Azure Blob Storage (planned)

## Solution Structure
```
StudioScheduler/
├── src/
│   ├── StudioScheduler.Client/           # Blazor WASM
│   ├── StudioScheduler.Server/           # ASP.NET Core API
│   ├── StudioScheduler.Core/             # Domain Models & Interfaces
│   ├── StudioScheduler.Infrastructure/    # Data Access & External Services
│   └── StudioScheduler.Shared/           # DTOs & Shared Models
├── tests/
│   ├── StudioScheduler.UnitTests/        # Unit tests
│   ├── StudioScheduler.IntegrationTests/ # EF Core integration tests
│   ├── StudioScheduler.PlaywrightTests/  # ✅ API & E2E tests (Playwright)
│   └── StudioScheduler.ReservationTests/ # Business logic tests
└── tools/                                # DB migrations, scripts
```

## Current Status

### ✅ Completed Features
- **Solution Structure**: Complete project setup with proper layered architecture
- **Entity Framework Integration**: SQLite database with complete schema
- **Repository Pattern**: Full CRUD operations for all entities
- **Data Migration**: Automatic seeding from JSON data to SQLite
- **Core API Endpoints**: Working endpoints for locations, rooms, classes, and schedules
- **Database Schema**: Complete with relationships and constraints
- **🎉 Playwright Integration Testing**: Modern .NET 9 API testing infrastructure with 19 comprehensive tests
  - Modern WebApplication pattern (bypasses WebApplicationFactory limitations)
  - Dynamic port assignment and database integration
  - Complete API contract validation and error handling testing
  - CI/CD ready with reliable execution

### 📊 Migrated Data
- **1 Location** with full studio details
- **2 Rooms** with equipment and capacity information
- **29 Dance Styles** (Salsa, Bachata, Zouk, Kizomba) - DanceClass entities
- **32 Class Groups** with recurring patterns - Schedule entities with levels and instructors
- **10 Students** with contact information
- **9 Student Passes** (Monthly/Annual with remaining classes)
- **23 Student Enrollments** across various class groups

### 🏗️ Current Architecture
The application uses Entity Framework Core with SQLite for data persistence, implementing a clean repository pattern with:
- **DanceClass**: Dance style definitions (Bachata, Salsa, etc.)
- **Schedule**: Actual class groups with levels, instructors, and time slots
- **Real-world business model**: Supports group progression, instructor changes, and group merging
- Async operations throughout
- Complex relationship queries
- Business logic methods for availability checks
- Comprehensive error handling

## Implementation Plan

### Phase 1: Core Infrastructure ✅ COMPLETED
1. ✅ Solution structure with proper layered architecture
2. ✅ Entity Framework Core setup with SQLite
3. ✅ Complete data models (User, Student, DanceClass as styles, Schedule as groups, Pass, Enrollment, etc.)
4. ✅ Repository pattern implementation
5. ✅ Data migration from JSON to database
6. ✅ Basic API endpoints working

### Phase 1.5: Current Development Focus 🔄
- API endpoints for student management
- Enrollment management endpoints for class groups
- Pass management system
- Authentication implementation

### Phase 2: Basic Backend Implementation
- API endpoints for CRUD operations on dance styles and class groups
- Business logic implementation for group management
- Service layer setup
- Basic error handling
- Logging setup

### Phase 3: Frontend Foundation
- Blazor project setup with MudBlazor
- Authentication UI
- Basic layouts and navigation
- Core components structure

### Phase 4: Feature Implementation
- Dance style management module
- Class group management with level progression
- User management module
- Pass system
- Real-time updates with SignalR

### Phase 5: Advanced Business Features
- Group merging and splitting capabilities
- Instructor substitution management
- Semester planning tools
- Student progression analytics

### Phase 6: Azure Infrastructure
- App Service setup
- Database deployment
- Application Insights integration
- CI/CD pipeline completion
