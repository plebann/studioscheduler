# Database Model - Studio Scheduler - 2025-06-20_23-44

## Entity Relationship Diagram

```mermaid
erDiagram
    Users {
        Guid Id PK
        string Email
        string FirstName
        string LastName
        string PasswordHash
        string Gender
        DateTime DateOfBirth
        UserRole Role
        DateTime CreatedAt
        DateTime UpdatedAt
        string Discriminator
    }
    
    Students {
        Guid Id PK
        string Phone
        bool IsActive
    }
    
    Locations {
        Guid Id PK
        string Name
        string Address
        string Description
        int Capacity
        TimeSpan OpeningTime
        TimeSpan ClosingTime
        bool IsActive
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Rooms {
        Guid Id PK
        string Name
        string Description
        int Capacity
        Guid LocationId FK
        bool IsActive
        string Equipment
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    DanceClasses {
        Guid Id PK
        string Name
        string Description
        string Style
        bool IsActive
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Schedules {
        Guid Id PK
        string Name
        Guid LocationId FK
        DateTime EffectiveFrom
        DateTime EffectiveTo
        bool IsActive
        Guid DanceClassId FK
        DayOfWeek DayOfWeek
        TimeSpan StartTime
        int Duration
        bool IsRecurring
        bool IsCancelled
        string Level
        Guid InstructorId FK
        Guid RoomId FK
        int Capacity
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Passes {
        Guid Id PK
        Guid UserId FK
        DateTime StartDate
        DateTime EndDate
        PassType Type
        int ClassesPerWeek
        int TotalClasses
        bool IsActive
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Enrollments {
        Guid Id PK
        Guid StudentId FK
        Guid ScheduleId FK
        DateTime EnrolledDate
        bool IsActive
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Attendances {
        Guid Id PK
        Guid StudentId FK
        Guid ScheduleId FK
        DateTime ClassDate
        bool WasPresent
        Guid PassUsed FK
        int PassClassNumber
        string Notes
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    %% Inheritance Relationship
    Users ||--o{ Students : "inherits (TPH)"
    
    %% Location Relationships
    Locations ||--o{ Rooms : "contains"
    Locations ||--o{ Schedules : "hosts"
    
    %% Room Relationships
    Rooms ||--o{ Schedules : "assigned to"
    
    %% User/Student Relationships
    Users ||--o{ Passes : "owns"
    Users ||--o{ Schedules : "instructs"
    Students ||--o{ Enrollments : "enrolls in"
    Students ||--o{ Attendances : "attends"
    
    %% DanceClass Relationships
    DanceClasses ||--o{ Schedules : "scheduled as"
    
    %% Schedule Relationships
    Schedules ||--o{ Enrollments : "has students"
    Schedules ||--o{ Attendances : "tracks attendance"
    
    %% Pass Relationships
    Passes ||--o{ Attendances : "used for"
```

## Business Rules and Constraints

### Entity Constraints

1. **User/Student Inheritance**
   - Uses Table-Per-Hierarchy (TPH) with Discriminator column
   - All Students are Users but not all Users are Students

2. **Location/Room Hierarchy**
   - Rooms belong to exactly one Location
   - Locations can have multiple Rooms
   - Schedules can specify both Location and specific Room

3. **Schedule Configuration**
   - Must have a Location and DanceClass
   - Instructor is optional (can be TBD)
   - Room is optional (can use general Location)
   - EffectiveTo is optional (ongoing schedules)

4. **Pass Business Logic**
   - 10 different PassTypes with unique rules
   - Complex weekly limits and validity periods
   - Rich domain methods for calculations

5. **Enrollment vs Attendance**
   - Enrollment: Student signs up for a recurring schedule
   - Attendance: Individual class participation records
   - Student can be enrolled but miss classes

### Delete Behavior

- **Cascade**: Student deletion removes all Passes, Enrollments, Attendances
- **Restrict**: Cannot delete Location/Room/Schedule with dependent records
- **Set Null**: Pass deletion nullifies Attendance.PassUsed references

### Key Features

1. **Audit Trail**: All entities have CreatedAt/UpdatedAt timestamps
2. **Soft Delete**: IsActive flags for logical deletion
3. **Guid Keys**: Distributed system ready with Guid primary keys
4. **Rich Domain**: Pass entity contains complex business logic
5. **Flexible Scheduling**: Support for recurring and one-time classes

## PassType Enumeration

```mermaid
graph TD
    A[PassType] --> B[SingleClass - 40 PLN]
    A --> C[Monthly1Course - 130 PLN]
    A --> D[Monthly2Courses - 200 PLN]
    A --> E[Monthly3Courses - 240 PLN]
    A --> F[Monthly4Courses - 280 PLN]
    A --> G[Monthly5Courses - 320 PLN]
    A --> H[Flexi4Classes - 140 PLN]
    A --> I[Flexi8Classes - 220 PLN]
    A --> J[FullPass - 350 PLN]
    
    B --> B1[1 class total, P2+ only]
    C --> C1[4 classes, 1/week, 28 days]
    D --> D1[8 classes, 2/week, 28 days]
    E --> E1[12 classes, 3/week, 28 days]
    F --> F1[16 classes, 4/week, 28 days]
    G --> G1[20 classes, 5/week, 28 days]
    H --> H1[4 classes, max 1/week, any type]
    I --> I1[8 classes, max 2/week, any type]
    J --> J1[Unlimited classes, 28 days]
```

## UserRole Enumeration

- **Admin**: Full system access
- **DeskPerson**: Front desk staff with limited admin access
- **Student**: Can book and attend classes

## Database Implementation Notes

- **Development**: SQLite database at `src/StudioScheduler.Server/studioscheduler.db`
- **Production**: PostgreSQL with Azure hosting
- **Migrations**: Entity Framework Core 9 with migration history
- **Performance**: Indexed foreign keys and common query patterns
- **Security**: Password hashing, role-based access control
