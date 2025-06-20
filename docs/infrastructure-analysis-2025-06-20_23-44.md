# Infrastructure Analysis - 2025-06-20_23-44

## Session Information
- **Timestamp**: 2025-06-20_23-44
- **Task**: Analyze Infrastructure project and prepare visual database model
- **Focus**: Database schema, entity relationships, and data access patterns

## Analysis Scope
- Review Core domain models
- Examine Infrastructure data layer
- Analyze Entity Framework configuration
- Map entity relationships
- Document database schema
- Create visual database model using Mermaid

## Project Structure
```
StudioScheduler.Infrastructure/
├── Data/ApplicationDbContext.cs
├── Repositories/ (various entity repositories)
├── Migrations/ (EF Core migrations)
├── Services/ (infrastructure services)
└── MockRepositories/ (test data)

StudioScheduler.Core/
├── Models/ (domain entities)
├── Interfaces/Repositories/ (repository contracts)
└── Enums/ (domain enums)
```

## Analysis Status
- [x] Session documentation created
- [x] Examine Core domain models
- [x] Review Infrastructure DbContext
- [x] Analyze entity relationships
- [x] Document migration history
- [x] Create visual database model
- [x] Generate final documentation

## Domain Models Analysis

### Core Entities

#### User (Base Entity)
- **Primary Key**: Guid Id
- **Required Fields**: Email, FirstName, LastName, PasswordHash
- **Optional Fields**: Gender, DateOfBirth
- **Enum**: UserRole (Admin, DeskPerson, Student)
- **Inheritance**: Student inherits from User (TPH strategy)

#### Student (Inherits from User)
- **Additional Fields**: Phone, IsActive
- **Navigation Properties**: 
  - Passes (One-to-Many)
  - Enrollments (One-to-Many)
  - AttendanceRecords (One-to-Many)
- **Business Logic**: CurrentPass property for active pass calculation

#### Location
- **Primary Key**: Guid Id
- **Required Fields**: Name, Address, Capacity, OpeningTime, ClosingTime
- **Optional Fields**: Description
- **Navigation Properties**: Rooms, Schedules

#### Room
- **Primary Key**: Guid Id
- **Foreign Key**: LocationId → Location
- **Required Fields**: Name, Capacity
- **Optional Fields**: Description, Equipment (List<string>)
- **Navigation Properties**: Schedules

#### DanceClass
- **Primary Key**: Guid Id
- **Required Fields**: Name, Description, Style
- **Business Logic**: IsActive property with automatic UpdatedAt
- **Navigation Properties**: Schedules

#### Schedule
- **Primary Key**: Guid Id
- **Foreign Keys**: 
  - LocationId → Location (Required)
  - DanceClassId → DanceClass (Required)
  - InstructorId → User (Optional)
  - RoomId → Room (Optional)
- **Required Fields**: Name, EffectiveFrom, DayOfWeek, StartTime, Duration, Level, Capacity
- **Optional Fields**: EffectiveTo, InstructorId, RoomId
- **Navigation Properties**: Enrollments

#### Pass
- **Primary Key**: Guid Id
- **Foreign Key**: UserId → User
- **Required Fields**: StartDate, EndDate, Type, ClassesPerWeek, TotalClasses
- **Enum**: PassType (10 different types including SingleClass, Monthly, Flexi, FullPass)
- **Rich Domain Logic**: 
  - CalculateRemainingClasses()
  - CanUseForClass()
  - GetPassStatus()
  - Complex weekly limits and validity calculations

#### Enrollment
- **Primary Key**: Guid Id
- **Foreign Keys**: 
  - StudentId → Student
  - ScheduleId → Schedule
- **Required Fields**: EnrolledDate
- **Purpose**: Links students to specific class schedules

#### Attendance
- **Primary Key**: Guid Id
- **Foreign Keys**: 
  - StudentId → Student
  - ScheduleId → Schedule
  - PassUsed → Pass (Optional)
- **Required Fields**: ClassDate, WasPresent
- **Optional Fields**: PassUsed, PassClassNumber, Notes
- **Purpose**: Tracks actual class attendance and pass usage

### Database Configuration (Entity Framework)

#### Inheritance Strategy
- **Table-Per-Hierarchy (TPH)**: User/Student using Discriminator column

#### Foreign Key Relationships
- **Cascade Delete**: Student → Passes, Enrollments, Attendances
- **Restrict Delete**: Location/Room relationships, Schedule relationships
- **Set Null**: Attendance → Pass (when pass is deleted)

#### Key Database Features
- **SQLite**: Development database
- **PostgreSQL**: Production database
- **Guid Primary Keys**: All entities use Guid for distributed system support
- **Audit Fields**: CreatedAt, UpdatedAt on all entities
- **Soft Delete**: IsActive flags for logical deletion

## Migration History Analysis

The database schema has evolved through 8 migrations since the initial creation:

### 1. **20250616083153_InitialCreate**
- Initial database schema creation
- Core entities: Users, Locations, Rooms, DanceClasses, Schedules, Passes, Enrollments, Attendances
- Basic relationships established

### 2. **20250616170251_RemoveReservations**
- Removed reservation system (simplified to enrollment-based approach)
- Aligned with KISS principle - removed unnecessary complexity

### 3. **20250616194143_UpdatePassToSalsaMeModel**
- Updated Pass entity to match SalsaMe business model
- Added PassType enum with 10 different pass types
- Implemented 28-day validity periods and weekly limits

### 4. **20250617103149_SyncModelWithDb**
- Synchronized model definitions with actual database state
- Ensured consistency between code and database schema

### 5. **20250619115454_ChangeDurationToMinutes**
- Changed Schedule.Duration from TimeSpan to int (minutes)
- Simplified duration storage and calculations

### 6. **20250619122350_RemoveRecurrencePattern**
- Removed complex recurrence pattern system
- Simplified to DayOfWeek + StartTime approach (KISS principle)

### 7. **20250619173142_RemoveRecurrenceEndDate**
- Removed RecurrenceEndDate field
- Uses EffectiveTo field for schedule validity periods

### 8. **20250101000000_MoveLevelAndInstructorToSchedule**
- Moved Level and InstructorId from DanceClass to Schedule
- Allows same dance class to have different levels and instructors
- Better separation of concerns (DanceClass = style definition, Schedule = actual implementation)

### Migration Evolution Insights

1. **Simplification Trend**: Multiple migrations removed complexity (reservations, recurrence patterns)
2. **Business Model Alignment**: Pass system evolved to match real SalsaMe pricing and rules
3. **Data Modeling Improvements**: Level/Instructor moved to Schedule for better flexibility
4. **KISS Principle Applied**: Complex systems replaced with simpler, more maintainable solutions

## Infrastructure Architecture Summary

### Repository Pattern Implementation
- **Interface Segregation**: Separate repository interfaces in Core project
- **Concrete Implementation**: Repository classes in Infrastructure project
- **Mock Support**: MockRepositories for testing and development

### Dependency Injection Configuration
- **DependencyInjection.cs**: Centralized service registration
- **Clean Architecture**: Infrastructure depends on Core, not vice versa
- **Testing Support**: Mockable repository interfaces

### Key Infrastructure Features

1. **Entity Framework Core 9**: Latest version with performance improvements
2. **Database Flexibility**: SQLite (dev) / PostgreSQL (prod) support
3. **Rich Domain Models**: Business logic embedded in entities (especially Pass)
4. **Audit Capabilities**: Comprehensive tracking with timestamps
5. **Scalable Design**: Guid keys and soft deletes for distributed systems

### Performance Considerations
- **Indexed Foreign Keys**: Automatic EF Core indexing
- **Lazy Loading**: Navigation properties configured for performance
- **Query Optimization**: Repository pattern enables query optimization
- **Caching Strategy**: Repository layer ready for caching implementation

### Security Implementation
- **Password Hashing**: Secure user authentication
- **Role-Based Access**: UserRole enum with three levels
- **Data Protection**: Restrict delete behavior prevents data loss
- **Audit Trail**: Complete tracking of data changes

This infrastructure successfully balances business complexity with technical simplicity, following Clean Architecture principles while maintaining the KISS and YAGNI philosophies specified in the .clinerules.
