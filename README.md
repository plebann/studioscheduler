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
│   ├── StudioScheduler.UnitTests/
│   └── StudioScheduler.IntegrationTests/
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

### 📊 Migrated Data
- **1 Location** with full studio details
- **2 Rooms** with equipment and capacity information
- **29 Dance Classes** (Salsa, Bachata, Zouk, Kizomba)
- **32 Class Schedules** with recurring patterns
- **10 Students** with contact information
- **9 Student Passes** (Monthly/Annual with remaining classes)
- **23 Student Enrollments** across various classes

### 🏗️ Current Architecture
The application uses Entity Framework Core with SQLite for data persistence, implementing a clean repository pattern with:
- Async operations throughout
- Complex relationship queries
- Business logic methods for availability checks
- Comprehensive error handling

## Implementation Plan

### Phase 1: Core Infrastructure ✅ COMPLETED
1. ✅ Solution structure with proper layered architecture
2. ✅ Entity Framework Core setup with SQLite
3. ✅ Complete data models (User, Student, DanceClass, Schedule, Pass, Enrollment, etc.)
4. ✅ Repository pattern implementation
5. ✅ Data migration from JSON to database
6. ✅ Basic API endpoints working

### Phase 1.5: Current Development Focus 🔄
- API endpoints for student management
- Enrollment management endpoints
- Pass management system
- Authentication implementation

### Phase 2: Basic Backend Implementation
- API endpoints for CRUD operations
- Business logic implementation
- Service layer setup
- Basic error handling
- Logging setup

### Phase 3: Frontend Foundation
- Blazor project setup with MudBlazor
- Authentication UI
- Basic layouts and navigation
- Core components structure

### Phase 4: Feature Implementation
- Class management module
- User management module
- Pass system
- Reservation system
- Real-time updates with SignalR

### Phase 5: Azure Infrastructure
- App Service setup
- Database deployment
- Application Insights integration
- CI/CD pipeline completion
