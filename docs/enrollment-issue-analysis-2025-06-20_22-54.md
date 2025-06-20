# Enrollment Duplicate Issue Analysis
**Date:** 2025-06-20_22-54  
**Analyst:** Cline  
**Issue:** Multiple enrollments created per pass purchase causing duplicate students in attendance lists

## Root Cause Identified

### Location of Issue
**File:** `src/StudioScheduler.Infrastructure/Services/PassService.cs`  
**Method:** `PurchasePassAsync` (lines 145-190)

### Problematic Code
```csharp
// Create enrollments for 4 consecutive weeks
var enrollments = new List<Enrollment>();

foreach (var scheduleId in selectedScheduleIds)
{
    for (int week = 0; week < 4; week++)  // ← THIS CREATES 4 ENROLLMENTS PER SCHEDULE
    {
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = DateTime.UtcNow,
            IsActive = true
        };
        enrollments.Add(enrollment);
    }
}
```

### Business Logic Behind 4 Enrollments
The original design assumed:
- **4-week pass validity** = 4 separate enrollment records
- Each enrollment represents one week of the 4-week pass
- **Intention:** Track weekly attendance separately

### Actual Business Reality
- **Pass:** Student purchases 4-week access to selected classes
- **Schedule:** Defines recurring weekly class (e.g., "Monday 7PM Bachata")
- **Enrollment:** Should represent student-schedule relationship (not weekly instances)
- **Attendance:** Should track individual class participation dates

### Impact on System
1. **AttendanceController.GetClassSchedule():**
   - Queries enrollments by schedule ID
   - Returns 4 enrollment records for same student
   - Creates 4 duplicate entries in attendance list

2. **Database:** 
   - Unnecessary data duplication
   - 4x storage for enrollment data
   - Performance impact on enrollment queries

3. **User Experience:**
   - Operators see same student 4 times in attendance lists
   - Confusing UI for marking attendance
   - Potential for data inconsistency

## Configuration Analysis
**No external configuration drives this behavior** - it's hardcoded in the business logic:
- `for (int week = 0; week < 4; week++)` - hardcoded 4-week assumption
- Comment confirms intention: "Create enrollments for 4 consecutive weeks"

## Data Model Issues

### Current Enrollment Model
```csharp
public class Enrollment
{
    public Guid Id { get; set; }
    public required Guid StudentId { get; set; }
    public required Guid ScheduleId { get; set; }
    public required DateTime EnrolledDate { get; set; }
    public bool IsActive { get; set; } = true;
    // No PassId reference - disconnected from pass
    // No week number or date range - all enrollments identical
}
```

### Issues with Current Model
1. **No Pass Reference:** Enrollments not linked to originating pass
2. **No Temporal Information:** All 4 enrollments are identical except ID
3. **No Business Logic:** Cannot distinguish between different enrollment purposes

## Solution Requirements

### Core Fix
Replace the 4-enrollment creation with single enrollment per student-schedule relationship:

```csharp
// Instead of 4 enrollments per schedule:
foreach (var scheduleId in selectedScheduleIds)
{
    var enrollment = new Enrollment
    {
        Id = Guid.NewGuid(),
        StudentId = studentId,
        ScheduleId = scheduleId,
        EnrolledDate = DateTime.UtcNow,
        IsActive = true
    };
    enrollments.Add(enrollment);
}
```

### Business Logic Alignment
- **Pass:** Manages validity period and usage rules
- **Enrollment:** Represents student eligibility for recurring class
- **Attendance:** Records actual participation with specific dates
- **Schedule:** Defines recurring class pattern

## Impact Assessment
**Files Requiring Changes:**
1. `PassService.PurchasePassAsync()` - Fix enrollment creation
2. `AttendanceController.GetClassSchedule()` - Ensure distinct students
3. Database migration - Consolidate existing duplicate enrollments
4. Enrollment repository - Add duplicate prevention

**Risk Level:** Medium
- Data migration required for existing duplicates
- Multiple system components affected
- Need to preserve attendance history integrity

## Repository Analysis - Critical Issues Found

### EnrollmentRepository Problems
**File:** `src/StudioScheduler.Infrastructure/Repositories/EnrollmentRepository.cs`

#### Issue 1: No Duplicate Prevention
```csharp
public async Task<Enrollment> CreateAsync(Enrollment enrollment)
{
    _context.Enrollments.Add(enrollment);  // No validation!
    await _context.SaveChangesAsync();
    return enrollment;
}

public async Task<IEnumerable<Enrollment>> CreateBatchAsync(IEnumerable<Enrollment> enrollments)
{
    _context.Enrollments.AddRange(enrollments);  // Allows duplicates!
    await _context.SaveChangesAsync();
    return enrollments;
}
```

**Problem:** Repository blindly creates enrollments without checking for existing student-schedule combinations.

#### Issue 2: Misleading Methods
```csharp
public async Task<Enrollment?> GetByStudentAndScheduleAsync(Guid studentId, Guid scheduleId)
{
    return await _context.Enrollments
        .FirstOrDefaultAsync(e => e.StudentId == studentId && e.ScheduleId == scheduleId);
}

public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid scheduleId)
{
    return await _context.Enrollments
        .AnyAsync(e => e.StudentId == studentId && e.ScheduleId == scheduleId);
}
```

**Problem:** These methods return/check for ANY enrollment, not considering that there might be 4 duplicates. `FirstOrDefaultAsync` returns random enrollment among duplicates.

#### Issue 3: Capacity Count Wrong
```csharp
public async Task<int> GetEnrollmentCountAsync(Guid scheduleId)
{
    return await _context.Enrollments.CountAsync(e => e.ScheduleId == scheduleId);
}
```

**Problem:** Counts all enrollments, so if 10 students have 4 enrollments each = 40 count instead of 10 students.

### AttendanceController Impact
Based on previous analysis of `AttendanceController.GetClassSchedule()`:
```csharp
var enrollments = await _enrollmentRepository.GetByScheduleIdAsync(scheduleGuid);
foreach (var enrollment in enrollments.Where(e => e.IsActive))
{
    // Creates one StudentAttendanceDto per enrollment
    // Result: 4 duplicate students in attendance list
}
```

## Complete Enrollment Flow Documentation

### 1. Pass Purchase Flow (Client → Server)
```
BuyPassModal.razor (Client)
    ↓ User selects pass type + schedules + start date
    ↓ Calls PassService.PurchasePassAsync()
PassController.purchase (Server)
    ↓ Validates request
    ↓ Calls PassService.PurchasePassAsync()
PassService.PurchasePassAsync()
    ↓ Creates Pass
    ↓ FOR EACH schedule: Creates 4 enrollments (PROBLEM!)
    ↓ Calls EnrollmentRepository.CreateBatchAsync()
EnrollmentRepository.CreateBatchAsync()
    ↓ No duplicate validation
    ↓ Saves all enrollments to database
```

### 2. Attendance Query Flow (Operator → UI)
```
AttendanceController.GetClassSchedule()
    ↓ Calls EnrollmentRepository.GetByScheduleIdAsync()
EnrollmentRepository.GetByScheduleIdAsync()
    ↓ Returns ALL enrollments for schedule (including 4 per student)
AttendanceController
    ↓ Creates StudentAttendanceDto for EACH enrollment
    ↓ Result: Same student appears 4 times in list
UI Display
    ↓ Operator sees duplicate students
```

### 3. ClassAttendanceService Impact
**Key Finding**: `ClassAttendanceService` does NOT handle enrollments directly:
- Only manages attendance marking and student search
- Uses `StudentRepository` and `AttendanceRepository`
- **No enrollment creation or querying** - problem isolated to PassService

### 4. Client-Side Pass Purchase
**BuyPassModal.razor findings**:
- User selects exactly N classes (based on pass type)
- Validates start date aligns with selected schedule days  
- Sends `BuyPassRequestDto` with `SelectedScheduleIds` list
- **Client has NO knowledge** of enrollment multiplication issue

## Comprehensive Fix Requirements

### Repository Layer
1. **Add duplicate prevention** in `CreateAsync`
2. **Modify methods** to return distinct students
3. **Add new method** `GetActiveByStudentAndScheduleAsync` 
4. **Fix capacity counting** to count distinct students

### Service Layer  
1. **Fix PassService** enrollment creation logic
2. **Update AttendanceController** to ensure distinct students
3. **Add enrollment lifecycle** management

### Database
1. **Migration script** to consolidate existing duplicates
2. **Add unique constraint** to prevent future duplicates
3. **Preserve attendance** history integrity

---
**Key Finding:** The issue is a fundamental misunderstanding of the enrollment concept, treating it as weekly instances rather than student-schedule relationships.

**Critical Discovery:** Repository layer has no duplicate prevention, making the problem systemic beyond just PassService.

## Database Schema Analysis - Major Issues

### Enrollment Table Schema
```sql
-- Current Enrollment table (from ModelSnapshot)
CREATE TABLE "Enrollments" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "EnrolledDate" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "ScheduleId" TEXT NOT NULL,
    "StudentId" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);

-- Indexes
CREATE INDEX "IX_Enrollments_ScheduleId" ON "Enrollments" ("ScheduleId");
CREATE INDEX "IX_Enrollments_StudentId" ON "Enrollments" ("StudentId");

-- Foreign Keys
FOREIGN KEY ("ScheduleId") REFERENCES "Schedules" ("Id") ON DELETE RESTRICT;
FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
```

### Critical Schema Problems

#### 1. **NO UNIQUE CONSTRAINT**
**Problem:** Database allows infinite duplicate enrollments for same student-schedule combination.
**Current:** No constraint preventing multiple enrollments
**Needed:** `UNIQUE (StudentId, ScheduleId)` for active enrollments

#### 2. **NO PASS REFERENCE**  
**Problem:** Enrollments completely disconnected from originating passes.
**Current:** No `PassId` column
**Impact:** Cannot track which pass created enrollment or manage enrollment lifecycle

#### 3. **WRONG DELETE BEHAVIOR**
**Current:** `ON DELETE CASCADE` for StudentId
**Problem:** Deleting student removes enrollment history
**Should be:** `ON DELETE RESTRICT` to preserve data integrity

### Pass-Enrollment Relationship Issues

#### Missing Connection
```csharp
// Current models have NO direct relationship
public class Pass { /* No enrollments collection */ }
public class Enrollment { /* No PassId reference */ }
```

**Business Impact:**
- Cannot track enrollment lifecycle with pass expiry
- Cannot prevent conflicting enrollments from different passes
- Cannot audit which pass created which enrollment

### Required Database Changes

#### 1. Add Unique Constraint
```sql
-- Prevent duplicate active enrollments
CREATE UNIQUE INDEX "IX_Enrollments_StudentId_ScheduleId_Active" 
ON "Enrollments" ("StudentId", "ScheduleId") 
WHERE "IsActive" = 1;
```

#### 2. Add Pass Reference
```sql
-- Link enrollment to originating pass
ALTER TABLE "Enrollments" ADD COLUMN "PassId" TEXT NULL;
CREATE INDEX "IX_Enrollments_PassId" ON "Enrollments" ("PassId");
ALTER TABLE "Enrollments" ADD CONSTRAINT "FK_Enrollments_Passes_PassId" 
FOREIGN KEY ("PassId") REFERENCES "Passes" ("Id") ON DELETE SET NULL;
```

#### 3. Fix Delete Behavior  
```sql
-- Change cascade to restrict for better data integrity
ALTER TABLE "Enrollments" DROP CONSTRAINT "FK_Enrollments_Users_StudentId";
ALTER TABLE "Enrollments" ADD CONSTRAINT "FK_Enrollments_Users_StudentId" 
FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE RESTRICT;
```

## Data Migration Requirements

### Current Database State Estimate
**Expected duplicate pattern:**
- Each student has ~4 enrollments per schedule (from 4-week passes)
- If 100 students with 2 classes each = 800 enrollment records (should be 200)
- Database storage: 4x excessive enrollment data

### Migration Strategy
1. **Identify duplicates:** Group by (StudentId, ScheduleId)
2. **Keep most recent:** `MAX(CreatedAt)` per group
3. **Delete duplicates:** Remove older enrollment records
4. **Preserve attendance:** Ensure all attendance records point to kept enrollment
5. **Add constraints:** Apply unique constraint after cleanup

## System-Wide Enrollment Usage Analysis (Task 1.6)

### Controllers Using Enrollments

#### 1. **AttendanceController** (PRIMARY IMPACT)
```csharp
var enrollments = await _enrollmentRepository.GetByScheduleIdAsync(scheduleGuid);
foreach (var enrollment in enrollments.Where(e => e.IsActive))
{
    // Creates duplicate StudentAttendanceDto entries
}
```
**Impact**: Direct cause of duplicate students in attendance lists.

#### 2. **SchedulesController** 
**Finding**: ✅ **NO enrollment dependencies** - only handles schedule CRUD operations.

#### 3. **StudentsController**
**Finding**: ✅ **NO enrollment dependencies** - only handles student CRUD operations.

#### 4. **PassController**
**Finding**: ✅ **NO direct enrollment queries** - delegates to PassService.

### Services Using Enrollments

#### 1. **PassService** (ENROLLMENT CREATOR)
```csharp
await _enrollmentRepository.CreateBatchAsync(enrollments); // Creates 4 per schedule
```
**Impact**: Root cause of enrollment multiplication.

#### 2. **ClassAttendanceService**
**Finding**: ✅ **NO enrollment handling** - uses StudentRepository and AttendanceRepository only.

### Repositories Using Enrollments

#### 1. **EnrollmentRepository** (CORE ISSUE)
**Methods with problems**:
- `GetByScheduleIdAsync()` - Returns ALL enrollments (including duplicates)
- `GetByStudentIdAsync()` - Returns ALL enrollments for student
- `GetByStudentAndScheduleAsync()` - Returns random duplicate
- `IsStudentEnrolledAsync()` - Checks ANY enrollment existence
- `GetEnrollmentCountAsync()` - Wrong capacity calculation

#### 2. **StudentRepository** (INDIRECT IMPACT)
```csharp
.Where(s => s.Enrollments.Any(e => e.ScheduleId == scheduleId))
```
**Impact**: Student queries using enrollment navigation properties may return wrong results due to duplicates.

### Data Seeding Impact
```csharp
// DataSeedingService.cs
_context.Enrollments.Add(enrollment); // Seeds enrollment data
```
**Impact**: May seed duplicate enrollments if run multiple times.

## API Endpoint Behavior Analysis (Task 1.7)

### Confirmed Duplicate Issue

#### **AttendanceController.GetClassSchedule() Endpoint**
```
GET /api/attendance/class/{scheduleId}
```

**Expected Behavior**: Return list of students enrolled in class.
**Actual Behavior**: Returns same student multiple times.

**Root Cause Flow**:
1. `_enrollmentRepository.GetByScheduleIdAsync(scheduleId)` returns 4 enrollments per student
2. `foreach (var enrollment in enrollments)` creates 4 `StudentAttendanceDto` objects per student
3. Response contains duplicate student entries

**Example Response Pattern**:
```json
{
  "enrolledStudents": [
    { "studentId": "abc", "firstName": "John", "lastName": "Doe" },
    { "studentId": "abc", "firstName": "John", "lastName": "Doe" },
    { "studentId": "abc", "firstName": "John", "lastName": "Doe" },
    { "studentId": "abc", "firstName": "John", "lastName": "Doe" }
  ]
}
```

### Secondary Impact Endpoints

#### **Pass Purchase Endpoints**
```
POST /api/pass/purchase
```
**Impact**: Creates 4 enrollments per selected schedule, perpetuating the problem.

#### **Student Queries by Schedule**
```csharp
// StudentRepository.GetByScheduleIdAsync()
.Where(s => s.Enrollments.Any(e => e.ScheduleId == scheduleId))
```
**Impact**: May return incorrect results due to enrollment duplicates in navigation properties.

## Test Case Documentation

### Reproduction Steps
1. **Student purchases 4-week pass**
   - Selects 2 classes (e.g., Monday Bachata, Wednesday Salsa)
   - Expected: 2 enrollments created
   - **Actual: 8 enrollments created (4 per class)**

2. **Operator opens attendance for Monday Bachata**
   - Calls: `GET /api/attendance/class/{mondayBachataScheduleId}`
   - Expected: Student appears once in list
   - **Actual: Student appears 4 times in list**

3. **Database State**
   - Expected: 1 enrollment per student per schedule
   - **Actual: 4 enrollments per student per schedule**

### Error Scenarios
- **Capacity Management**: Class appears "full" when it's not (counts 40 enrollments instead of 10 students)
- **Attendance Marking**: Operator confusion when seeing duplicate students
- **Pass Validation**: Complex logic needed to handle multiple enrollments per student

---
**Database Finding:** Schema has fundamental flaws enabling and perpetuating the duplicate enrollment problem.

**System-Wide Finding:** The enrollment duplication issue is isolated to PassService (creation) and AttendanceController (display), with no impact on other controllers or services.
