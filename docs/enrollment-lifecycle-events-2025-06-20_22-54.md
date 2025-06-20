# Enrollment Lifecycle Events Design
**Date:** 2025-06-20_22-54  
**Project:** StudioScheduler - Enrollment Lifecycle Management  
**Phase:** 2.4 - Define Enrollment Lifecycle Events

## Enrollment Lifecycle Overview

### **Enrollment States**
```
CREATED → ACTIVE → INACTIVE → [REACTIVATED → ACTIVE]
   ↑         ↑        ↑           ↑
   |         |        |           |
Pass      Pass      Pass      New Pass
Purchase  Active    Expires   Purchase
```

### **Core Lifecycle Events**
1. **Enrollment Creation** (Pass Purchase)
2. **Enrollment Activation** (Pass Becomes Active)
3. **Enrollment Deactivation** (Pass Expires/Deactivated)
4. **Enrollment Reactivation** (New Pass Purchase)
5. **Enrollment Extension** (Pass Extension/New Pass)

## Event 1: Enrollment Creation on Pass Purchase

### **Trigger**: Student purchases pass and selects schedules

### **Event Flow**
```
1. User completes pass purchase with selected schedules
2. PassService.PurchasePassAsync() triggered
3. For each selected schedule:
   a. Check if enrollment exists (active or inactive)
   b. If exists: Reactivate existing enrollment
   c. If not: Create new enrollment
4. Link enrollment to originating pass (PassId)
5. Set enrollment as active
6. Persist changes to database
```

### **Implementation**
```csharp
public class EnrollmentCreationEvent
{
    public Guid PassId { get; set; }
    public Guid StudentId { get; set; }
    public List<Guid> ScheduleIds { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime Timestamp { get; set; }
}

public async Task<List<Enrollment>> HandleEnrollmentCreation(EnrollmentCreationEvent evt)
{
    var createdEnrollments = new List<Enrollment>();
    
    foreach (var scheduleId in evt.ScheduleIds)
    {
        // Check for existing enrollment
        var existingEnrollment = await _enrollmentRepository
            .GetByStudentAndScheduleAsync(evt.StudentId, scheduleId);
        
        if (existingEnrollment != null)
        {
            // REACTIVATE existing enrollment
            var reactivated = await HandleEnrollmentReactivation(new EnrollmentReactivationEvent
            {
                EnrollmentId = existingEnrollment.Id,
                PassId = evt.PassId,
                StartDate = evt.StartDate,
                Timestamp = evt.Timestamp
            });
            
            createdEnrollments.Add(reactivated);
        }
        else
        {
            // CREATE new enrollment
            var newEnrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = evt.StudentId,
                ScheduleId = scheduleId,
                PassId = evt.PassId,
                EnrolledDate = evt.StartDate,
                IsActive = true,
                CreatedAt = evt.Timestamp
            };
            
            var created = await _enrollmentRepository.CreateAsync(newEnrollment);
            createdEnrollments.Add(created);
            
            // Publish enrollment created event
            await _eventPublisher.PublishAsync(new EnrollmentCreatedEvent
            {
                EnrollmentId = created.Id,
                StudentId = evt.StudentId,
                ScheduleId = scheduleId,
                PassId = evt.PassId,
                Timestamp = evt.Timestamp
            });
        }
    }
    
    return createdEnrollments;
}
```

### **Validation Rules**
```csharp
public async Task ValidateEnrollmentCreation(EnrollmentCreationEvent evt)
{
    // Rule 2.4.1: Student must exist and be active
    var student = await _studentRepository.GetByIdAsync(evt.StudentId);
    if (student == null || !student.IsActive)
        throw new InvalidOperationException("Student not found or inactive");
    
    // Rule 2.4.2: All schedules must exist and be active
    foreach (var scheduleId in evt.ScheduleIds)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null || !schedule.IsActive)
            throw new InvalidOperationException($"Schedule {scheduleId} not found or inactive");
    }
    
    // Rule 2.4.3: Pass must be valid and active
    var pass = await _passRepository.GetByIdAsync(evt.PassId);
    if (pass == null || !pass.IsActive)
        throw new InvalidOperationException("Pass not found or inactive");
}
```

## Event 2: Enrollment Deactivation on Pass Expiry

### **Trigger**: Pass expires or is manually deactivated

### **Event Flow**
```
1. Pass expires (EndDate < Today) OR Pass.IsActive = false
2. System identifies all enrollments linked to expired pass
3. For each affected enrollment:
   a. Check if student has other active passes
   b. If yes: Link enrollment to alternative pass
   c. If no: Deactivate enrollment
4. Update enrollment status and timestamps
5. Notify affected services/components
```

### **Implementation**
```csharp
public class EnrollmentDeactivationEvent
{
    public Guid PassId { get; set; }
    public string Reason { get; set; } // "Expired", "Deactivated", "Cancelled"
    public DateTime Timestamp { get; set; }
}

public async Task HandleEnrollmentDeactivation(EnrollmentDeactivationEvent evt)
{
    // Find all active enrollments linked to expired pass
    var affectedEnrollments = await _context.Enrollments
        .Where(e => e.PassId == evt.PassId && e.IsActive)
        .ToListAsync();
    
    foreach (var enrollment in affectedEnrollments)
    {
        // Check if student has other active passes
        var alternativePasses = await _passRepository
            .GetActivePassesForStudent(enrollment.StudentId);
        
        var validAlternative = alternativePasses
            .Where(p => p.Id != evt.PassId && p.IsActive)
            .OrderByDescending(p => p.EndDate)
            .FirstOrDefault();
        
        if (validAlternative != null)
        {
            // TRANSFER to alternative pass
            enrollment.PassId = validAlternative.Id;
            enrollment.UpdatedAt = evt.Timestamp;
            
            await _eventPublisher.PublishAsync(new EnrollmentTransferredEvent
            {
                EnrollmentId = enrollment.Id,
                FromPassId = evt.PassId,
                ToPassId = validAlternative.Id,
                Timestamp = evt.Timestamp
            });
        }
        else
        {
            // DEACTIVATE enrollment
            enrollment.IsActive = false;
            enrollment.UpdatedAt = evt.Timestamp;
            
            await _eventPublisher.PublishAsync(new EnrollmentDeactivatedEvent
            {
                EnrollmentId = enrollment.Id,
                StudentId = enrollment.StudentId,
                ScheduleId = enrollment.ScheduleId,
                Reason = evt.Reason,
                Timestamp = evt.Timestamp
            });
        }
    }
    
    await _context.SaveChangesAsync();
}
```

### **Automated Pass Expiry Check**
```csharp
public class PassExpiryService
{
    public async Task CheckAndHandleExpiredPasses()
    {
        var expiredPasses = await _passRepository
            .GetPassesExpiredAsOf(DateTime.UtcNow);
        
        foreach (var expiredPass in expiredPasses)
        {
            // Mark pass as inactive
            expiredPass.IsActive = false;
            expiredPass.UpdatedAt = DateTime.UtcNow;
            
            // Trigger enrollment deactivation
            await HandleEnrollmentDeactivation(new EnrollmentDeactivationEvent
            {
                PassId = expiredPass.Id,
                Reason = "Expired",
                Timestamp = DateTime.UtcNow
            });
        }
        
        await _context.SaveChangesAsync();
    }
}
```

## Event 3: Enrollment Extension on New Pass Purchase

### **Trigger**: Student purchases new pass for schedules they're already enrolled in

### **Event Flow**
```
1. Student purchases new pass
2. Selected schedules include schedules student is already enrolled in
3. System identifies existing enrollments
4. Extend/update existing enrollments instead of creating duplicates
5. Link enrollments to new pass for better benefits
6. Update enrollment dates and validity
```

### **Implementation**
```csharp
public class EnrollmentExtensionEvent
{
    public Guid ExistingEnrollmentId { get; set; }
    public Guid NewPassId { get; set; }
    public Guid PreviousPassId { get; set; }
    public DateTime NewStartDate { get; set; }
    public DateTime Timestamp { get; set; }
}

public async Task HandleEnrollmentExtension(EnrollmentExtensionEvent evt)
{
    var enrollment = await _enrollmentRepository.GetByIdAsync(evt.ExistingEnrollmentId);
    if (enrollment == null) return;
    
    var newPass = await _passRepository.GetByIdAsync(evt.NewPassId);
    var previousPass = await _passRepository.GetByIdAsync(evt.PreviousPassId);
    
    // Determine if new pass provides better benefits
    bool shouldUpdateToNewPass = DeterminePassPriority(newPass, previousPass);
    
    if (shouldUpdateToNewPass)
    {
        // Update enrollment to reference new pass
        enrollment.PassId = evt.NewPassId;
        enrollment.EnrolledDate = evt.NewStartDate;
        enrollment.UpdatedAt = evt.Timestamp;
        
        // Ensure enrollment is active
        if (!enrollment.IsActive)
        {
            enrollment.IsActive = true;
        }
        
        await _enrollmentRepository.UpdateAsync(enrollment);
        
        // Publish extension event
        await _eventPublisher.PublishAsync(new EnrollmentExtendedEvent
        {
            EnrollmentId = enrollment.Id,
            StudentId = enrollment.StudentId,
            ScheduleId = enrollment.ScheduleId,
            PreviousPassId = evt.PreviousPassId,
            NewPassId = evt.NewPassId,
            ExtensionDate = evt.NewStartDate,
            Timestamp = evt.Timestamp
        });
    }
}

private bool DeterminePassPriority(Pass newPass, Pass existingPass)
{
    // Priority rules:
    // 1. Longer validity period
    // 2. More total classes
    // 3. More recent purchase
    
    if (newPass.EndDate > existingPass.EndDate) return true;
    if (newPass.TotalClasses > existingPass.TotalClasses) return true;
    if (newPass.CreatedAt > existingPass.CreatedAt) return true;
    
    return false;
}
```

## Event 4: Enrollment Reactivation

### **Trigger**: Student purchases pass for schedules with inactive enrollments

### **Event Flow**
```
1. Student purchases new pass
2. System finds inactive enrollment for selected schedule
3. Reactivate existing enrollment instead of creating new one
4. Update enrollment with new pass reference and dates
5. Preserve enrollment history and attendance records
```

### **Implementation**
```csharp
public class EnrollmentReactivationEvent
{
    public Guid EnrollmentId { get; set; }
    public Guid PassId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime Timestamp { get; set; }
}

public async Task<Enrollment> HandleEnrollmentReactivation(EnrollmentReactivationEvent evt)
{
    var enrollment = await _enrollmentRepository.GetByIdAsync(evt.EnrollmentId);
    if (enrollment == null)
        throw new InvalidOperationException($"Enrollment {evt.EnrollmentId} not found");
    
    // Reactivate enrollment
    enrollment.IsActive = true;
    enrollment.PassId = evt.PassId;
    enrollment.EnrolledDate = evt.StartDate;
    enrollment.UpdatedAt = evt.Timestamp;
    
    await _enrollmentRepository.UpdateAsync(enrollment);
    
    // Publish reactivation event
    await _eventPublisher.PublishAsync(new EnrollmentReactivatedEvent
    {
        EnrollmentId = enrollment.Id,
        StudentId = enrollment.StudentId,
        ScheduleId = enrollment.ScheduleId,
        PassId = evt.PassId,
        ReactivationDate = evt.StartDate,
        Timestamp = evt.Timestamp
    });
    
    return enrollment;
}
```

## Event 5: Enrollment Validation and Cleanup

### **Trigger**: Periodic system maintenance or pass status changes

### **Event Flow**
```
1. System periodically validates enrollment integrity
2. Identify orphaned enrollments (no backing passes)
3. Identify invalid enrollments (expired passes)
4. Clean up inconsistent data
5. Generate health reports
```

### **Implementation**
```csharp
public class EnrollmentValidationEvent
{
    public DateTime ValidationDate { get; set; }
    public string ValidationReason { get; set; }
}

public async Task<EnrollmentHealthReport> HandleEnrollmentValidation(EnrollmentValidationEvent evt)
{
    var report = new EnrollmentHealthReport
    {
        ValidationDate = evt.ValidationDate,
        TotalEnrollments = await _context.Enrollments.CountAsync(),
        ActiveEnrollments = await _context.Enrollments.CountAsync(e => e.IsActive)
    };
    
    // Find orphaned enrollments (no PassId)
    var orphanedEnrollments = await _context.Enrollments
        .Where(e => e.IsActive && e.PassId == null)
        .ToListAsync();
    
    report.OrphanedEnrollments = orphanedEnrollments.Count;
    
    // Find enrollments with expired passes
    var enrollmentsWithExpiredPasses = await _context.Enrollments
        .Include(e => e.Pass)
        .Where(e => e.IsActive 
                 && e.Pass != null 
                 && (!e.Pass.IsActive || e.Pass.EndDate < DateTime.UtcNow))
        .ToListAsync();
    
    report.InvalidEnrollments = enrollmentsWithExpiredPasses.Count;
    
    // Cleanup orphaned enrollments
    foreach (var orphaned in orphanedEnrollments)
    {
        orphaned.IsActive = false;
        orphaned.UpdatedAt = evt.ValidationDate;
    }
    
    // Cleanup invalid enrollments
    foreach (var invalid in enrollmentsWithExpiredPasses)
    {
        // Check if student has alternative active passes
        var hasAlternativePasses = await _passRepository
            .HasActivePassesForStudent(invalid.StudentId);
        
        if (!hasAlternativePasses)
        {
            invalid.IsActive = false;
            invalid.UpdatedAt = evt.ValidationDate;
        }
    }
    
    report.CleanedUpEnrollments = orphanedEnrollments.Count + 
                                 enrollmentsWithExpiredPasses.Count(e => !e.IsActive);
    
    await _context.SaveChangesAsync();
    
    return report;
}
```

## Event Publishing and Handling

### **Event Infrastructure**
```csharp
public interface IEnrollmentEventPublisher
{
    Task PublishAsync<T>(T enrollmentEvent) where T : class;
}

public class EnrollmentEventPublisher : IEnrollmentEventPublisher
{
    private readonly IServiceProvider _serviceProvider;
    
    public async Task PublishAsync<T>(T enrollmentEvent) where T : class
    {
        var handlers = _serviceProvider.GetServices<IEnrollmentEventHandler<T>>();
        
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(enrollmentEvent);
        }
    }
}

public interface IEnrollmentEventHandler<T>
{
    Task HandleAsync(T enrollmentEvent);
}
```

### **Event Handlers Examples**
```csharp
// Audit logging handler
public class EnrollmentAuditHandler : 
    IEnrollmentEventHandler<EnrollmentCreatedEvent>,
    IEnrollmentEventHandler<EnrollmentDeactivatedEvent>,
    IEnrollmentEventHandler<EnrollmentReactivatedEvent>
{
    public async Task HandleAsync(EnrollmentCreatedEvent evt)
    {
        await LogAuditEvent("ENROLLMENT_CREATED", evt);
    }
    
    public async Task HandleAsync(EnrollmentDeactivatedEvent evt)
    {
        await LogAuditEvent("ENROLLMENT_DEACTIVATED", evt);
    }
    
    public async Task HandleAsync(EnrollmentReactivatedEvent evt)
    {
        await LogAuditEvent("ENROLLMENT_REACTIVATED", evt);
    }
}

// Notification handler
public class EnrollmentNotificationHandler : 
    IEnrollmentEventHandler<EnrollmentDeactivatedEvent>
{
    public async Task HandleAsync(EnrollmentDeactivatedEvent evt)
    {
        // Notify student about enrollment deactivation
        await _notificationService.NotifyStudentAsync(
            evt.StudentId, 
            "Your enrollment has expired. Please purchase a new pass to continue.");
    }
}

// Cache invalidation handler
public class EnrollmentCacheHandler : 
    IEnrollmentEventHandler<EnrollmentCreatedEvent>,
    IEnrollmentEventHandler<EnrollmentDeactivatedEvent>
{
    public async Task HandleAsync(EnrollmentCreatedEvent evt)
    {
        await _cacheService.InvalidateEnrollmentCacheAsync(evt.StudentId, evt.ScheduleId);
    }
    
    public async Task HandleAsync(EnrollmentDeactivatedEvent evt)
    {
        await _cacheService.InvalidateEnrollmentCacheAsync(evt.StudentId, evt.ScheduleId);
    }
}
```

## Integration with Existing System

### **PassService Integration**
```csharp
// Updated PassService.PurchasePassAsync()
public async Task<Pass> PurchasePassAsync(Guid studentId, PassType passType, 
                                         DateTime startDate, List<Guid> selectedScheduleIds)
{
    // ... existing validation logic ...
    
    var pass = new Pass { /* ... */ };
    var createdPass = await _passRepository.AddAsync(pass);
    
    // Use new enrollment lifecycle events
    var enrollmentCreationEvent = new EnrollmentCreationEvent
    {
        PassId = createdPass.Id,
        StudentId = studentId,
        ScheduleIds = selectedScheduleIds,
        StartDate = startDate,
        Timestamp = DateTime.UtcNow
    };
    
    await _enrollmentLifecycleService.HandleEnrollmentCreation(enrollmentCreationEvent);
    
    return createdPass;
}
```

### **Scheduled Tasks Integration**
```csharp
// Background service for periodic validation
public class EnrollmentMaintenanceService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Check for expired passes every hour
                await _passExpiryService.CheckAndHandleExpiredPasses();
                
                // Validate enrollment integrity daily
                if (DateTime.UtcNow.Hour == 2) // 2 AM
                {
                    var validationEvent = new EnrollmentValidationEvent
                    {
                        ValidationDate = DateTime.UtcNow,
                        ValidationReason = "Scheduled maintenance"
                    };
                    
                    await _enrollmentLifecycleService.HandleEnrollmentValidation(validationEvent);
                }
                
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in enrollment maintenance service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
```

## Compliance with Core Principles

### **KISS (Keep It Simple)**
- Clear event definitions with single responsibilities
- Simple event handling logic
- Straightforward lifecycle state transitions

### **YAGNI (You Aren't Gonna Need It)**
- Focus only on essential lifecycle events
- No speculative event types
- Minimal complexity in event handling

### **SRP (Single Responsibility Principle)**
- Each event handles one specific lifecycle transition
- Separate handlers for different concerns (audit, notification, etc.)
- Clear separation between event publishing and handling

### **DRY (Don't Repeat Yourself)**
- Reusable event infrastructure
- Common validation patterns
- Shared event handling patterns

---
**Status:** Task 2.4 Complete ✅  
**Phase 2: Business Logic Design COMPLETE ✅**  
**Next:** Phase 3 - Implementation
