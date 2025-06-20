# Pass-Enrollment Relationship Design
**Date:** 2025-06-20_22-54  
**Project:** StudioScheduler - Pass-Enrollment Integration  
**Phase:** 2.3 - Design Pass-Enrollment Relationship

## Current State Analysis

### **Existing Issues**
```
PROBLEM: Enrollments are completely disconnected from passes
- No PassId reference in Enrollment table
- No way to track which pass created which enrollment
- No enrollment lifecycle management based on pass validity
- Cannot audit enrollment sources or validate enrollment eligibility
```

### **Business Impact**
- **Orphaned Enrollments**: Active enrollments with no backing passes
- **Invalid Enrollments**: Students appear enrolled but have no valid pass
- **Audit Problems**: Cannot track enrollment history or pass usage
- **Capacity Issues**: Cannot properly validate enrollment eligibility

## Proposed Relationship Model

### **Core Relationship Design**
```
Pass (1) ←→ (N) Enrollments
- Pass can have multiple enrollments (one per selected schedule)
- Enrollment can be linked to multiple passes over time (reactivation)
- Pass validity determines enrollment validity
- Enrollment lifecycle managed by pass lifecycle
```

## PassId Reference Implementation

### **Decision: YES - Enrollment NEEDS PassId Reference**

#### **Justification**
1. **Enrollment Validity**: Must validate enrollment against active passes
2. **Audit Trail**: Track which pass created/maintains each enrollment
3. **Business Logic**: Pass expiry should affect enrollment status
4. **Data Integrity**: Prevent orphaned enrollments
5. **User Experience**: Display pass information in enrollment context

#### **Database Schema Changes**
```sql
-- Add PassId reference to Enrollment table
ALTER TABLE Enrollments ADD PassId TEXT NULL;

-- Add foreign key constraint
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Passes_PassId 
FOREIGN KEY (PassId) REFERENCES Passes (Id) ON DELETE SET NULL;

-- Add index for performance
CREATE INDEX IX_Enrollments_PassId ON Enrollments (PassId);
```

#### **Entity Model Updates**
```csharp
// Enhanced Enrollment model
public class Enrollment
{
    public Guid Id { get; set; }
    public required Guid StudentId { get; set; }
    public required Guid ScheduleId { get; set; }
    public required DateTime EnrolledDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // NEW: Pass reference
    public Guid? PassId { get; set; }  // Nullable for historical data
    
    // Navigation properties
    public Student Student { get; set; } = null!;
    public Schedule Schedule { get; set; } = null!;
    public Pass? Pass { get; set; }  // Navigation to originating pass
}

// Enhanced Pass model
public class Pass
{
    // ... existing properties ...
    
    // NEW: Navigation to created enrollments
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
```

## Enrollment Expiry When Pass Expires

### **Pass Expiry Scenarios**

#### **Scenario 2.3.A: Single Pass Expires**
```
GIVEN: Student has 1 active pass covering Schedule X
WHEN: Pass expires (EndDate < Today OR IsActive = false)
THEN: Deactivate enrollment for Schedule X
LOGIC: Enrollment.IsActive = false (no backing passes remain)
```

#### **Scenario 2.3.B: Multiple Passes - One Expires**
```
GIVEN: Student has 2 active passes covering Schedule X
WHEN: Pass A expires, Pass B remains active
THEN: Keep enrollment active (backed by Pass B)
LOGIC: Enrollment remains valid while ANY pass covers the schedule
```

#### **Scenario 2.3.C: All Passes Expire**
```
GIVEN: Student has multiple passes covering Schedule X
WHEN: All passes expire
THEN: Deactivate enrollment for Schedule X
LOGIC: No active passes remain to back the enrollment
```

### **Enrollment Validity Logic**
```csharp
public static class EnrollmentValidator
{
    public static async Task<bool> IsEnrollmentValid(
        Enrollment enrollment, 
        IPassRepository passRepository)
    {
        // Check if student has ANY active pass covering this schedule
        var activePasses = await passRepository.GetActivePassesForStudent(
            enrollment.StudentId);
        
        // Enrollment is valid if ANY active pass exists for the student
        // (Pass usage rules are enforced separately)
        return activePasses.Any(p => p.IsActive 
                                  && p.StartDate <= DateTime.UtcNow 
                                  && p.EndDate >= DateTime.UtcNow);
    }
    
    public static async Task ValidateAndUpdateEnrollmentStatus(
        Enrollment enrollment, 
        IPassRepository passRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        var isValid = await IsEnrollmentValid(enrollment, passRepository);
        
        if (!isValid && enrollment.IsActive)
        {
            // Deactivate enrollment - no backing passes
            enrollment.IsActive = false;
            enrollment.UpdatedAt = DateTime.UtcNow;
            await enrollmentRepository.UpdateAsync(enrollment);
        }
        else if (isValid && !enrollment.IsActive)
        {
            // Reactivate enrollment - backing pass found
            enrollment.IsActive = true;
            enrollment.UpdatedAt = DateTime.UtcNow;
            await enrollmentRepository.UpdateAsync(enrollment);
        }
    }
}
```

## Multiple Passes for Same Schedule Scenarios

### **Scenario 2.3.1: Overlapping Pass Periods**
```
SITUATION: Student purchases Monthly2 pass, then buys Flexi4 pass 
          before Monthly2 expires, both covering Schedule X

BUSINESS RULE: Single enrollment remains active
IMPLEMENTATION:
- Keep existing enrollment active
- Update PassId to reference newest/most beneficial pass
- Enrollment validity backed by EITHER pass
- Pass usage rules enforced independently
```

```csharp
public async Task HandleOverlappingPasses(
    Guid studentId, Guid scheduleId, Guid newPassId, Guid existingPassId)
{
    var enrollment = await _enrollmentRepository
        .GetActiveByStudentAndScheduleAsync(studentId, scheduleId);
    
    if (enrollment != null)
    {
        // Determine which pass provides better benefits
        var newPass = await _passRepository.GetByIdAsync(newPassId);
        var existingPass = await _passRepository.GetByIdAsync(existingPassId);
        
        // Update to more beneficial pass (e.g., longer duration, more classes)
        if (IsMoreBeneficial(newPass, existingPass))
        {
            enrollment.PassId = newPassId;
            enrollment.UpdatedAt = DateTime.UtcNow;
            await _enrollmentRepository.UpdateAsync(enrollment);
        }
    }
}

private bool IsMoreBeneficial(Pass newPass, Pass existingPass)
{
    // Business logic to determine better pass
    return newPass.EndDate > existingPass.EndDate || 
           newPass.TotalClasses > existingPass.TotalClasses;
}
```

### **Scenario 2.3.2: Sequential Pass Purchases**
```
SITUATION: Student's Monthly2 pass expires, then buys new Flexi8 pass
          covering same Schedule X

BUSINESS RULE: Reactivate existing enrollment
IMPLEMENTATION:
- Find existing (inactive) enrollment for Schedule X
- Reactivate enrollment and link to new pass
- Update enrollment date to new pass start date
```

```csharp
public async Task HandleSequentialPassPurchase(
    Guid studentId, Guid scheduleId, Guid newPassId, DateTime startDate)
{
    // Look for ANY enrollment (active or inactive)
    var existingEnrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.StudentId == studentId 
                                && e.ScheduleId == scheduleId);
    
    if (existingEnrollment != null)
    {
        // Reactivate existing enrollment
        existingEnrollment.IsActive = true;
        existingEnrollment.PassId = newPassId;
        existingEnrollment.EnrolledDate = startDate;
        existingEnrollment.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
    }
    else
    {
        // Create new enrollment
        var newEnrollment = new Enrollment
        {
            StudentId = studentId,
            ScheduleId = scheduleId,
            PassId = newPassId,
            EnrolledDate = startDate,
            IsActive = true
        };
        
        await _enrollmentRepository.CreateAsync(newEnrollment);
    }
}
```

### **Scenario 2.3.3: Different Pass Types for Same Schedule**
```
SITUATION: Student has FullPass (unlimited) and Monthly1 (1 class/week)
          both covering Schedule X

BUSINESS RULE: Single enrollment, use most permissive pass
IMPLEMENTATION:
- Single enrollment linked to FullPass (more permissive)
- Pass usage validation uses most restrictive applicable pass
- Enrollment validity backed by EITHER pass
```

## Pass-Enrollment Lifecycle Integration

### **Pass Purchase → Enrollment Creation/Reactivation**
```csharp
public async Task<List<Enrollment>> CreateEnrollmentsForPass(
    Guid passId, Guid studentId, List<Guid> scheduleIds, DateTime startDate)
{
    var enrollments = new List<Enrollment>();
    
    foreach (var scheduleId in scheduleIds)
    {
        // Check for existing enrollment (active or inactive)
        var existingEnrollment = await _enrollmentRepository
            .GetByStudentAndScheduleAsync(studentId, scheduleId);
        
        if (existingEnrollment != null)
        {
            // Reactivate existing enrollment
            if (!existingEnrollment.IsActive)
            {
                existingEnrollment.IsActive = true;
                existingEnrollment.EnrolledDate = startDate;
            }
            existingEnrollment.PassId = passId;  // Link to new pass
            existingEnrollment.UpdatedAt = DateTime.UtcNow;
            
            await _enrollmentRepository.UpdateAsync(existingEnrollment);
            enrollments.Add(existingEnrollment);
        }
        else
        {
            // Create new enrollment
            var newEnrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ScheduleId = scheduleId,
                PassId = passId,
                EnrolledDate = startDate,
                IsActive = true
            };
            
            var created = await _enrollmentRepository.CreateAsync(newEnrollment);
            enrollments.Add(created);
        }
    }
    
    return enrollments;
}
```

### **Pass Expiry → Enrollment Validation**
```csharp
public async Task HandlePassExpiry(Guid expiredPassId)
{
    // Find all enrollments linked to expired pass
    var affectedEnrollments = await _context.Enrollments
        .Where(e => e.PassId == expiredPassId && e.IsActive)
        .ToListAsync();
    
    foreach (var enrollment in affectedEnrollments)
    {
        // Check if student has other active passes
        var hasOtherActivePasses = await _passRepository
            .HasActivePassesForStudent(enrollment.StudentId);
        
        if (!hasOtherActivePasses)
        {
            // No other passes - deactivate enrollment
            enrollment.IsActive = false;
            enrollment.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Find best alternative pass to link enrollment to
            var activePasses = await _passRepository
                .GetActivePassesForStudent(enrollment.StudentId);
            
            var bestPass = activePasses
                .OrderByDescending(p => p.EndDate)
                .First();
            
            enrollment.PassId = bestPass.Id;
            enrollment.UpdatedAt = DateTime.UtcNow;
        }
    }
    
    await _context.SaveChangesAsync();
}
```

### **Pass Deactivation → Enrollment Review**
```csharp
public async Task HandlePassDeactivation(Guid deactivatedPassId)
{
    // Similar to expiry but immediate deactivation
    await HandlePassExpiry(deactivatedPassId);
}
```

## Data Migration Strategy

### **Existing Enrollments → Pass References**
```sql
-- Migration script to add PassId references to existing enrollments
UPDATE Enrollments 
SET PassId = (
    SELECT p.Id 
    FROM Passes p 
    WHERE p.UserId = Enrollments.StudentId 
      AND p.IsActive = 1
      AND p.StartDate <= Enrollments.EnrolledDate
      AND p.EndDate >= Enrollments.EnrolledDate
    ORDER BY p.CreatedAt DESC
    LIMIT 1
)
WHERE PassId IS NULL;

-- Handle enrollments that couldn't be linked to passes
UPDATE Enrollments 
SET IsActive = 0, UpdatedAt = CURRENT_TIMESTAMP
WHERE PassId IS NULL AND IsActive = 1;
```

### **Duplicate Enrollment Consolidation**
```sql
-- Before adding unique constraint, consolidate duplicates
WITH DuplicateEnrollments AS (
    SELECT StudentId, ScheduleId, MIN(Id) as KeepId
    FROM Enrollments 
    WHERE IsActive = 1
    GROUP BY StudentId, ScheduleId
    HAVING COUNT(*) > 1
)
UPDATE Enrollments 
SET IsActive = 0, UpdatedAt = CURRENT_TIMESTAMP
WHERE Id NOT IN (SELECT KeepId FROM DuplicateEnrollments)
  AND (StudentId, ScheduleId) IN (
      SELECT StudentId, ScheduleId FROM DuplicateEnrollments
  );
```

## Enrollment Validity Queries

### **Active Enrollment with Pass Validation**
```csharp
public async Task<IEnumerable<Enrollment>> GetValidEnrollmentsForSchedule(Guid scheduleId)
{
    return await _context.Enrollments
        .Where(e => e.ScheduleId == scheduleId 
                 && e.IsActive
                 && e.Pass != null
                 && e.Pass.IsActive
                 && e.Pass.StartDate <= DateTime.UtcNow
                 && e.Pass.EndDate >= DateTime.UtcNow)
        .Include(e => e.Student)
        .Include(e => e.Pass)
        .ToListAsync();
}
```

### **Student Enrollment Status Check**
```csharp
public async Task<bool> IsStudentValidlyEnrolled(Guid studentId, Guid scheduleId)
{
    var enrollment = await _context.Enrollments
        .Include(e => e.Pass)
        .FirstOrDefaultAsync(e => e.StudentId == studentId 
                               && e.ScheduleId == scheduleId 
                               && e.IsActive);
    
    if (enrollment?.Pass == null) return false;
    
    return enrollment.Pass.IsActive 
        && enrollment.Pass.StartDate <= DateTime.UtcNow 
        && enrollment.Pass.EndDate >= DateTime.UtcNow;
}
```

## Performance Considerations

### **Optimized Pass-Enrollment Queries**
```csharp
// Efficient enrollment validation
public async Task<Dictionary<Guid, bool>> ValidateMultipleEnrollments(
    List<Guid> enrollmentIds)
{
    var results = await _context.Enrollments
        .Where(e => enrollmentIds.Contains(e.Id))
        .Select(e => new 
        { 
            e.Id, 
            IsValid = e.IsActive 
                   && e.Pass != null 
                   && e.Pass.IsActive 
                   && e.Pass.StartDate <= DateTime.UtcNow 
                   && e.Pass.EndDate >= DateTime.UtcNow 
        })
        .ToDictionaryAsync(x => x.Id, x => x.IsValid);
    
    return results;
}
```

## Compliance with Core Principles

### **KISS (Keep It Simple)**
- Clear Pass → Enrollment relationship
- Simple validity rules based on pass status
- Straightforward reactivation logic

### **YAGNI (You Aren't Gonna Need It)**
- Focus on essential pass-enrollment linking
- No complex enrollment state machines
- Minimal enrollment lifecycle complexity

### **SRP (Single Responsibility Principle)**
- Pass: Manages access rights and validity periods
- Enrollment: Manages student-schedule relationships
- Validation: Separate service for enrollment validity

### **DRY (Don't Repeat Yourself)**
- Centralized enrollment validity logic
- Reusable pass-enrollment validation methods
- Single source of truth for enrollment status

---
**Status:** Task 2.3 Complete ✅  
**Next:** Task 2.4 - Define enrollment lifecycle events
