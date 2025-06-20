# Duplicate Prevention Logic Design
**Date:** 2025-06-20_22-54  
**Project:** StudioScheduler - Enrollment Duplicate Prevention  
**Phase:** 2.2 - Design Duplicate Prevention Logic

## Core Prevention Strategy

### **Multi-Layer Defense Approach**
```
Layer 1: Database Constraints (Physical Prevention)
Layer 2: Repository Validation (Data Access Prevention)  
Layer 3: Service Logic Validation (Business Logic Prevention)
Layer 4: Application Monitoring (Detection & Alerting)
```

## Layer 1: Database Constraints

### **Primary Constraint: Unique Index**
```sql
-- Prevent duplicate active enrollments at database level
CREATE UNIQUE INDEX IX_Enrollments_StudentId_ScheduleId_Active 
ON Enrollments (StudentId, ScheduleId) 
WHERE IsActive = 1;

-- This constraint ensures database-level duplicate prevention
-- Only ONE active enrollment per (StudentId, ScheduleId) combination
```

### **Additional Database Safeguards**
```sql
-- Foreign key constraints with proper delete behavior
ALTER TABLE Enrollments DROP CONSTRAINT FK_Enrollments_Users_StudentId;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Users_StudentId 
FOREIGN KEY (StudentId) REFERENCES Users (Id) ON DELETE RESTRICT;

-- Add PassId reference for enrollment tracking
ALTER TABLE Enrollments ADD PassId TEXT NULL;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Passes_PassId 
FOREIGN KEY (PassId) REFERENCES Passes (Id) ON DELETE SET NULL;
```

## Layer 2: Repository Validation

### **EnrollmentRepository Enhancement**

#### **New Method: GetActiveByStudentAndScheduleAsync**
```csharp
public async Task<Enrollment?> GetActiveByStudentAndScheduleAsync(Guid studentId, Guid scheduleId)
{
    return await _context.Enrollments
        .FirstOrDefaultAsync(e => e.StudentId == studentId 
                                && e.ScheduleId == scheduleId 
                                && e.IsActive);
}
```

#### **Enhanced CreateAsync with Duplicate Prevention**
```csharp
public async Task<Enrollment> CreateAsync(Enrollment enrollment)
{
    // VALIDATION RULE 2.2.1: Check for existing active enrollment
    var existingEnrollment = await GetActiveByStudentAndScheduleAsync(
        enrollment.StudentId, enrollment.ScheduleId);
    
    if (existingEnrollment != null)
    {
        throw new DuplicateEnrollmentException(
            $"Active enrollment already exists for Student {enrollment.StudentId} " +
            $"and Schedule {enrollment.ScheduleId}");
    }
    
    _context.Enrollments.Add(enrollment);
    await _context.SaveChangesAsync();
    return enrollment;
}
```

#### **Enhanced CreateOrReactivateAsync Method**
```csharp
public async Task<Enrollment> CreateOrReactivateAsync(Enrollment newEnrollment)
{
    // VALIDATION RULE 2.2.2: Check for ANY existing enrollment (active or inactive)
    var existingEnrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.StudentId == newEnrollment.StudentId 
                                && e.ScheduleId == newEnrollment.ScheduleId);
    
    if (existingEnrollment != null)
    {
        // REACTIVATION LOGIC: Update existing enrollment
        existingEnrollment.IsActive = true;
        existingEnrollment.EnrolledDate = newEnrollment.EnrolledDate;
        existingEnrollment.PassId = newEnrollment.PassId;
        existingEnrollment.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return existingEnrollment;
    }
    else
    {
        // CREATE NEW: No existing enrollment found
        return await CreateAsync(newEnrollment);
    }
}
```

## Layer 3: Service Logic Validation

### **PassService Enhanced Logic**

#### **Fixed PurchasePassAsync Method**
```csharp
public async Task<Pass> PurchasePassAsync(Guid studentId, PassType passType, 
                                         DateTime startDate, List<Guid> selectedScheduleIds)
{
    // Existing validation logic...
    
    // Create the pass
    var pass = new Pass { /* ... existing logic */ };
    
    // FIXED ENROLLMENT CREATION: One enrollment per schedule
    var enrollments = new List<Enrollment>();
    
    foreach (var scheduleId in selectedScheduleIds)
    {
        // VALIDATION RULE 2.2.3: Create or reactivate single enrollment
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ScheduleId = scheduleId,
            EnrolledDate = startDate,
            PassId = pass.Id,  // Link to originating pass
            IsActive = true
        };
        
        // Use enhanced repository method
        var createdEnrollment = await _enrollmentRepository.CreateOrReactivateAsync(enrollment);
        enrollments.Add(createdEnrollment);
    }
    
    var createdPass = await _passRepository.AddAsync(pass);
    return createdPass;
}
```

### **Validation Rules in Service Layer**

#### **Rule 2.2.4: Pre-Purchase Validation**
```csharp
private async Task ValidateEnrollmentEligibility(Guid studentId, List<Guid> scheduleIds)
{
    foreach (var scheduleId in scheduleIds)
    {
        // Check if student already has active enrollment
        var existingEnrollment = await _enrollmentRepository
            .GetActiveByStudentAndScheduleAsync(studentId, scheduleId);
            
        if (existingEnrollment != null)
        {
            // Check if enrollment is backed by active pass
            var hasActivePasses = await _passRepository
                .HasActivePassesForStudent(studentId);
                
            if (hasActivePasses)
            {
                throw new InvalidOperationException(
                    $"Student already enrolled in schedule {scheduleId} with active pass");
            }
        }
    }
}
```

## Behavior When Duplicate Enrollment Attempted

### **Scenario 2.2.A: Duplicate Creation via API**
```
ATTEMPT: Direct enrollment creation with existing active enrollment
DETECTION: Repository.CreateAsync validation
RESPONSE: Throw DuplicateEnrollmentException
CLIENT ACTION: Display user-friendly error message
```

### **Scenario 2.2.B: Duplicate via Pass Purchase**
```
ATTEMPT: Pass purchase for schedules student is already enrolled in
DETECTION: Service layer pre-validation
RESPONSE: Use CreateOrReactivateAsync to reactivate existing enrollment
CLIENT ACTION: Success - enrollment reactivated
```

### **Scenario 2.2.C: Concurrent Enrollment Creation**
```
ATTEMPT: Two simultaneous requests to create enrollment
DETECTION: Database unique constraint violation
RESPONSE: Second request fails with constraint violation
CLIENT ACTION: Retry with reactivation logic
```

### **Scenario 2.2.D: Database Constraint Violation**
```
ATTEMPT: Bypass application logic, direct database insert
DETECTION: Database unique index constraint
RESPONSE: Database throws constraint violation error
APPLICATION: Handle gracefully, check for reactivation
```

## Edge Cases & Handling

### **Edge Case 2.2.1: Expired Enrollment Reactivation**
```csharp
// GIVEN: Student has expired enrollment for Schedule X
// WHEN: Student purchases new pass for Schedule X
// THEN: Reactivate existing enrollment

public async Task<Enrollment> HandleExpiredEnrollmentReactivation(
    Guid studentId, Guid scheduleId, Guid newPassId, DateTime startDate)
{
    var expiredEnrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.StudentId == studentId 
                                && e.ScheduleId == scheduleId 
                                && !e.IsActive);
    
    if (expiredEnrollment != null)
    {
        // REACTIVATE: Update expired enrollment
        expiredEnrollment.IsActive = true;
        expiredEnrollment.EnrolledDate = startDate;
        expiredEnrollment.PassId = newPassId;
        expiredEnrollment.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return expiredEnrollment;
    }
    
    return null; // No expired enrollment found
}
```

### **Edge Case 2.2.2: Schedule Changes During Active Enrollment**
```csharp
// GIVEN: Student enrolled in Schedule X (Monday 7PM Bachata)
// WHEN: Schedule X changes to Monday 8PM Bachata
// THEN: Enrollment remains valid (same Schedule entity)

// NO ACTION REQUIRED: Enrollment references Schedule.Id
// Schedule changes (time, location) don't affect enrollment validity
```

### **Edge Case 2.2.3: Student Transfer Between Schedules**
```csharp
// GIVEN: Student enrolled in Schedule X
// WHEN: Student wants to transfer to Schedule Y mid-pass
// THEN: Deactivate old enrollment, create new enrollment

public async Task<Enrollment> TransferStudentToNewSchedule(
    Guid studentId, Guid fromScheduleId, Guid toScheduleId, Guid passId)
{
    // Deactivate old enrollment
    var oldEnrollment = await GetActiveByStudentAndScheduleAsync(studentId, fromScheduleId);
    if (oldEnrollment != null)
    {
        oldEnrollment.IsActive = false;
        oldEnrollment.UpdatedAt = DateTime.UtcNow;
    }
    
    // Create new enrollment
    var newEnrollment = new Enrollment
    {
        StudentId = studentId,
        ScheduleId = toScheduleId,
        PassId = passId,
        EnrolledDate = DateTime.UtcNow,
        IsActive = true
    };
    
    return await CreateOrReactivateAsync(newEnrollment);
}
```

### **Edge Case 2.2.4: Multiple Passes Covering Same Schedule**
```csharp
// GIVEN: Student has Flexi4 pass covering Schedule X
// WHEN: Student purchases Monthly2 pass also covering Schedule X
// THEN: Keep single enrollment active, link to newest pass

public async Task HandleMultiplePassesForSchedule(
    Guid studentId, Guid scheduleId, Guid newPassId)
{
    var existingEnrollment = await GetActiveByStudentAndScheduleAsync(studentId, scheduleId);
    
    if (existingEnrollment != null)
    {
        // UPDATE: Link to newest pass (may provide better benefits)
        existingEnrollment.PassId = newPassId;
        existingEnrollment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
    // If no existing enrollment, create new one normally
}
```

## Layer 4: Application Monitoring

### **Detection & Alerting**
```csharp
// MONITORING RULE 2.2.5: Detect enrollment anomalies
public async Task<EnrollmentHealthReport> GenerateEnrollmentHealthReport()
{
    var duplicateCount = await _context.Enrollments
        .GroupBy(e => new { e.StudentId, e.ScheduleId })
        .Where(g => g.Count(e => e.IsActive) > 1)
        .CountAsync();
    
    var orphanedCount = await _context.Enrollments
        .Where(e => e.IsActive && e.PassId == null)
        .CountAsync();
    
    return new EnrollmentHealthReport
    {
        DuplicateActiveEnrollments = duplicateCount,
        OrphanedEnrollments = orphanedCount,
        HealthStatus = duplicateCount == 0 ? "Healthy" : "Issues Detected"
    };
}
```

### **Automated Cleanup**
```csharp
// CLEANUP RULE 2.2.6: Periodic validation and cleanup
public async Task PerformEnrollmentValidation()
{
    // Find enrollments without backing active passes
    var orphanedEnrollments = await _context.Enrollments
        .Where(e => e.IsActive && !_context.Passes
            .Any(p => p.UserId == e.StudentId 
                   && p.IsActive 
                   && p.StartDate <= DateTime.UtcNow 
                   && p.EndDate >= DateTime.UtcNow))
        .ToListAsync();
    
    // Deactivate orphaned enrollments
    foreach (var enrollment in orphanedEnrollments)
    {
        enrollment.IsActive = false;
        enrollment.UpdatedAt = DateTime.UtcNow;
    }
    
    await _context.SaveChangesAsync();
}
```

## Exception Handling

### **Custom Exception Types**
```csharp
public class DuplicateEnrollmentException : Exception
{
    public Guid StudentId { get; }
    public Guid ScheduleId { get; }
    
    public DuplicateEnrollmentException(string message, Guid studentId, Guid scheduleId) 
        : base(message)
    {
        StudentId = studentId;
        ScheduleId = scheduleId;
    }
}

public class EnrollmentValidationException : Exception
{
    public EnrollmentValidationException(string message) : base(message) { }
}
```

### **Error Response Handling**
```csharp
// In controllers/middleware
public async Task<IActionResult> HandleEnrollmentCreation()
{
    try
    {
        // Enrollment creation logic
    }
    catch (DuplicateEnrollmentException ex)
    {
        return BadRequest(new 
        { 
            error = "Duplicate enrollment detected",
            message = ex.Message,
            studentId = ex.StudentId,
            scheduleId = ex.ScheduleId,
            suggestion = "Student is already enrolled in this class"
        });
    }
    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Enrollments_StudentId_ScheduleId_Active") == true)
    {
        return Conflict(new 
        { 
            error = "Enrollment conflict",
            message = "Cannot create duplicate enrollment due to database constraint"
        });
    }
}
```

## Performance Considerations

### **Optimized Queries**
```csharp
// PERFORMANCE RULE 2.2.7: Efficient duplicate checking
public async Task<bool> HasActiveEnrollmentOptimized(Guid studentId, Guid scheduleId)
{
    // Use EXISTS query for better performance
    return await _context.Enrollments
        .AnyAsync(e => e.StudentId == studentId 
                    && e.ScheduleId == scheduleId 
                    && e.IsActive);
}
```

### **Batch Validation**
```csharp
// For multiple schedule enrollment validation
public async Task<Dictionary<Guid, bool>> CheckMultipleEnrollmentsAsync(
    Guid studentId, List<Guid> scheduleIds)
{
    var existingEnrollments = await _context.Enrollments
        .Where(e => e.StudentId == studentId 
                 && scheduleIds.Contains(e.ScheduleId) 
                 && e.IsActive)
        .Select(e => e.ScheduleId)
        .ToListAsync();
    
    return scheduleIds.ToDictionary(
        scheduleId => scheduleId,
        scheduleId => existingEnrollments.Contains(scheduleId)
    );
}
```

## Compliance with Core Principles

### **KISS (Keep It Simple)**
- Clear validation rules at each layer
- Simple exception handling
- Straightforward reactivation logic

### **YAGNI (You Aren't Gonna Need It)**
- Focus only on preventing duplicates
- No speculative validation features
- Minimal complexity in edge case handling

### **SRP (Single Responsibility Principle)**
- Repository: Data access and basic validation
- Service: Business logic validation
- Database: Physical constraint enforcement

### **DRY (Don't Repeat Yourself)**
- Centralized validation methods
- Reusable duplicate checking logic
- Single source of truth for validation rules

---
**Status:** Task 2.2 Complete ✅  
**Next:** Task 2.3 - Design pass-enrollment relationship
