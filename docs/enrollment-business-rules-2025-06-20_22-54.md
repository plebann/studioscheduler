# Enrollment Business Rules Design
**Date:** 2025-06-20_22-54  
**Project:** StudioScheduler - Single Enrollment Business Logic  
**Phase:** 2.1 - Define Single Enrollment Business Rules

## Core Business Rule: One Active Enrollment Per Student Per Schedule

### **Primary Rule Definition**
```
RULE: At any given time, there SHALL be exactly ONE active enrollment 
      per student per schedule combination.

CONSTRAINT: (StudentId, ScheduleId, IsActive=true) MUST be unique
```

### **Business Justification**
- **Enrollment** represents the relationship between a student and a recurring weekly class
- **Pass** represents the purchased access period and usage rules
- **Attendance** represents individual class participation instances
- **Schedule** represents the recurring weekly class definition

## Enrollment Validity Period (Linked to Pass)

### **Validity Rules**

#### **Rule 2.1.1: Enrollment Activation**
```
WHEN: Pass is purchased and schedules are selected
THEN: Create ONE enrollment per selected schedule
WHERE: Enrollment.IsActive = true
AND: Enrollment.EnrolledDate = Pass.StartDate
```

#### **Rule 2.1.2: Enrollment Validity Period**
```
VALID PERIOD: Enrollment is valid while ANY associated pass is active
LOGIC: Enrollment.IsValid = EXISTS(Pass WHERE Pass.UserId = Student.Id 
                                  AND Pass.IsActive = true 
                                  AND Pass.StartDate <= TODAY 
                                  AND Pass.EndDate >= TODAY)
```

#### **Rule 2.1.3: Enrollment Expiry**
```
WHEN: ALL associated passes expire or become inactive
THEN: Enrollment.IsActive = false
BUT: Enrollment record is preserved for historical data
```

## Enrollment Reactivation Scenarios

### **Scenario 2.1.A: Student Buys New Pass for Same Schedule**
```
GIVEN: Student has expired/inactive enrollment for Schedule X
WHEN: Student purchases new pass and selects Schedule X
THEN: 
  IF existing enrollment exists (active or inactive)
    THEN reactivate existing enrollment
    AND update Enrollment.EnrolledDate = new Pass.StartDate
  ELSE create new enrollment
```

### **Scenario 2.1.B: Student Has Multiple Passes**
```
GIVEN: Student has active Pass A (expires in 1 week)
WHEN: Student purchases Pass B (starts today) for same schedules
THEN: 
  Keep single enrollment active
  Enrollment validity determined by ANY active pass
  No duplicate enrollments created
```

### **Scenario 2.1.C: Schedule Changes During Pass**
```
GIVEN: Student has active enrollment for Schedule X
WHEN: Schedule X is modified (time/location change)
THEN: 
  Enrollment remains active (references same Schedule entity)
  Student continues to be enrolled automatically
```

### **Scenario 2.1.D: Student Switches Schedules Mid-Pass**
```
GIVEN: Student has active enrollment for Schedule X
WHEN: Student wants to switch to Schedule Y mid-pass
THEN: 
  Deactivate enrollment for Schedule X
  Create new enrollment for Schedule Y
  Business rule: Pass usage rules still apply
```

## Implementation Rules

### **Database Constraints**
```sql
-- Unique constraint for active enrollments
CREATE UNIQUE INDEX IX_Enrollments_StudentId_ScheduleId_Active 
ON Enrollments (StudentId, ScheduleId) 
WHERE IsActive = 1;

-- PassId reference for enrollment tracking
ALTER TABLE Enrollments ADD PassId TEXT NULL;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Passes_PassId 
FOREIGN KEY (PassId) REFERENCES Passes (Id) ON DELETE SET NULL;
```

### **Application Logic Rules**

#### **Rule 2.1.4: Enrollment Creation Validation**
```csharp
BEFORE creating enrollment:
1. Check if active enrollment exists for (StudentId, ScheduleId)
2. IF exists THEN reactivate existing enrollment
3. ELSE create new enrollment
4. Link enrollment to originating pass (PassId)
```

#### **Rule 2.1.5: Pass Expiry Handling**
```csharp
WHEN pass expires:
1. Check if student has other active passes for same schedules
2. IF no active passes THEN deactivate enrollments
3. ELSE keep enrollments active (covered by other passes)
```

#### **Rule 2.1.6: Enrollment Query Rules**
```csharp
FOR attendance queries:
1. Only return active enrollments (IsActive = true)
2. GROUP BY StudentId to ensure distinct students
3. Validate enrollment is backed by active pass before showing in UI
```

## Pass-Enrollment Relationship

### **Relationship Model**
```
Pass (1) ←→ (N) Enrollments
- Pass.Id → Enrollment.PassId (foreign key)
- Multiple passes can reference same enrollment (reactivation)
- Enrollment validity determined by ANY active pass
```

### **Lifecycle Integration**
```
Pass Purchase → Enrollment Creation/Reactivation
Pass Expiry → Enrollment Validation (keep if other passes active)
Pass Deactivation → Check enrollment validity
Enrollment Deletion → Never (preserve for historical data)
```

## Edge Cases & Business Rules

### **Edge Case 2.1.A: Orphaned Enrollments**
```
SITUATION: Enrollment exists but no active passes
RULE: Enrollment.IsActive = false (automatically via validation)
ACTION: Deactivate enrollment but preserve record
```

### **Edge Case 2.1.B: Pass Purchased After Enrollment Exists**
```
SITUATION: Student manually enrolled, then buys pass for same schedule
RULE: Reactivate existing enrollment, link to new pass
ACTION: Update Enrollment.PassId and Enrollment.EnrolledDate
```

### **Edge Case 2.1.C: Multiple Pass Types for Same Schedule**
```
SITUATION: Student has Flexi4 pass and Monthly2 pass both covering Schedule X
RULE: Single enrollment remains active
VALIDATION: Pass usage rules still apply independently
```

### **Edge Case 2.1.D: Schedule Deletion**
```
SITUATION: Schedule is deleted while students are enrolled
RULE: Cascade delete enrollments (lose historical data) OR
ALTERNATIVE: Set Schedule as inactive, preserve enrollments
RECOMMENDATION: Preserve enrollments, mark Schedule as inactive
```

## Validation Rules Summary

### **Pre-Creation Validation**
1. Student must exist and be active
2. Schedule must exist and be active
3. Check for existing enrollment (active or inactive)
4. Validate pass eligibility for schedule

### **Post-Creation Validation**
1. Verify unique constraint not violated
2. Confirm enrollment is backed by active pass
3. Update enrollment validity status

### **Ongoing Validation**
1. Daily/periodic check of enrollment validity against active passes
2. Automatic deactivation of enrollments with no backing passes
3. Maintenance of data integrity

## Migration Strategy for Existing Data

### **Current State → Target State**
```
CURRENT: 4 enrollments per student per schedule (identical)
TARGET: 1 enrollment per student per schedule

MIGRATION LOGIC:
1. Group existing enrollments by (StudentId, ScheduleId)
2. Keep enrollment with MAX(CreatedAt) 
3. Delete duplicate enrollments
4. Preserve attendance history (update foreign keys if needed)
5. Add PassId references where possible
```

## Compliance with Core Principles

### **KISS (Keep It Simple)**
- Single enrollment per relationship
- Clear, understandable business rules
- Minimal complexity in validation logic

### **YAGNI (You Aren't Gonna Need It)**
- No speculative features
- Focus only on fixing duplicate issue
- Avoid over-engineering enrollment lifecycle

### **SRP (Single Responsibility Principle)**
- Enrollment: Student-Schedule relationship
- Pass: Access period and usage rules
- Attendance: Individual class participation

### **DRY (Don't Repeat Yourself)**
- Single source of truth for enrollment data
- No duplicate student-schedule relationships
- Centralized enrollment validation logic

---
**Status:** Task 2.1 Complete ✅  
**Next:** Task 2.2 - Design duplicate prevention logic
