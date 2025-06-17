# Real-World Architecture: Dance Studio Business Model

## Overview
Studio Scheduler is designed around real-world dance studio operations, where the business model reflects actual teaching practices and group dynamics.

## Core Architecture

### DanceClass (Dance Style Definition)
**Purpose**: Defines dance styles/types that the studio offers
- **Static entities**: "Bachata", "Salsa Cubana", "Salsa on1", "Zouk", "Kizomba"
- **Style categorization**: BACHATA, SALSA, ZOUK, KIZOMBA, etc.
- **Reusable templates**: Multiple class groups can use the same dance style

**Properties**:
```csharp
public class DanceClass
{
    public Guid Id { get; set; }
    public required string Name { get; init; }        // "Bachata", "Salsa Cubana"
    public required string Description { get; init; } // Description of the dance style
    public required string Style { get; init; }       // "BACHATA", "SALSA", "ZOUK"
    public bool IsActive { get; set; } = true;
}
```

### Schedule (Class Group)
**Purpose**: Represents actual class groups with specific characteristics
- **Dynamic entities**: Groups evolve over time
- **Level progression**: P1 → P2 → P3 → S1 → S2 → S3
- **Instructor assignment**: Can change due to availability, substitution, etc.
- **Time and location**: Specific weekly schedule in a particular room

**Properties**:
```csharp
public class Schedule
{
    public Guid Id { get; set; }
    public required string Name { get; set; }         // "Bachata P2 - Monday 18:00"
    
    public required Guid DanceClassId { get; set; }   // Reference to dance style
    public DanceClass? DanceClass { get; set; }
    
    public required string Level { get; set; }        // P1, P2, P3, S1, S2, S3
    public required Guid InstructorId { get; set; }   // Current instructor
    public User? Instructor { get; set; }
    public required Guid RoomId { get; set; }         // Room where class is held
    public Room? Room { get; set; }
    
    public required DateTime StartTime { get; set; }  // Weekly time slot
    public required TimeSpan Duration { get; set; }   // Class duration
    public bool IsRecurring { get; set; } = true;     // Weekly recurring
    
    public bool IsActive { get; set; } = true;
}
```

## Real-World Business Scenarios

### Scenario 1: Adding New Bachata P1 Classes
**Business Need**: Studio wants to offer more beginner Bachata classes due to high demand.

**Implementation**:
```csharp
// 1. Get existing Bachata dance style (or create if doesn't exist)
var bachataStyle = await _danceClassService.GetByStyleAsync("BACHATA").FirstOrDefault();
if (bachataStyle == null)
{
    bachataStyle = new DanceClass
    {
        Name = "Bachata",
        Description = "Sensual Bachata dance style",
        Style = "BACHATA"
    };
    await _danceClassService.CreateAsync(bachataStyle);
}

// 2. Create multiple class groups for the same style
var mondayGroup = new Schedule
{
    Name = "Bachata P1 - Monday 18:00",
    DanceClassId = bachataStyle.Id,
    Level = "P1",
    InstructorId = mariaId,
    RoomId = studioAId,
    StartTime = new DateTime(2025, 1, 6, 18, 0, 0), // Monday 18:00
    Duration = TimeSpan.FromMinutes(55)
};

var wednesdayGroup = new Schedule
{
    Name = "Bachata P1 - Wednesday 19:00",
    DanceClassId = bachataStyle.Id,
    Level = "P1",
    InstructorId = johnId,
    RoomId = studioBId,
    StartTime = new DateTime(2025, 1, 8, 19, 0, 0), // Wednesday 19:00
    Duration = TimeSpan.FromMinutes(55)
};
```

### Scenario 2: Class Group Progression
**Business Need**: After 3 months, the Bachata P1 group has progressed and should become P2.

**Implementation**:
```csharp
// 1. Update the existing group's level
var bachataP1Group = await _scheduleService.GetByIdAsync(groupId);
bachataP1Group.Level = "P2";
bachataP1Group.Name = "Bachata P2 - Monday 18:00";
bachataP1Group.UpdatedAt = DateTime.UtcNow;

await _scheduleService.UpdateAsync(bachataP1Group);

// 2. Optionally change instructor if needed
if (newInstructorId != bachataP1Group.InstructorId)
{
    bachataP1Group.InstructorId = newInstructorId;
    bachataP1Group.Name = $"Bachata P2 - Monday 18:00 - {newInstructorName}";
    await _scheduleService.UpdateAsync(bachataP1Group);
}
```

### Scenario 3: Group Merging During Semester Reorganization
**Business Need**: At the end of semester, small groups need to be merged to optimize resources.

**Implementation**:
```csharp
public async Task MergeSmallGroupsAsync()
{
    // 1. Find groups with low enrollment
    var allGroups = await _scheduleService.GetAllAsync();
    var smallGroups = new List<Schedule>();
    
    foreach (var group in allGroups.Where(g => g.IsActive))
    {
        var enrollmentCount = await _enrollmentService.GetEnrollmentCountAsync(group.Id);
        if (enrollmentCount < 5) // Threshold for small groups
        {
            smallGroups.Add(group);
        }
    }
    
    // 2. Group by similar characteristics (same dance style, similar level)
    var groupsByStyle = smallGroups.GroupBy(g => g.DanceClassId);
    
    foreach (var styleGroup in groupsByStyle)
    {
        var groups = styleGroup.ToList();
        if (groups.Count >= 2)
        {
            // 3. Create new merged group
            var newGroup = new Schedule
            {
                Name = $"Merged {groups[0].DanceClass.Name} - {DateTime.Now:MMM yyyy}",
                DanceClassId = groups[0].DanceClassId,
                Level = DetermineOptimalLevel(groups), // Business logic
                InstructorId = SelectBestInstructor(groups), // Business logic
                RoomId = SelectOptimalRoom(groups), // Business logic
                StartTime = DetermineOptimalTime(groups), // Business logic
                Duration = groups[0].Duration
            };
            
            var createdGroup = await _scheduleService.CreateAsync(newGroup);
            
            // 4. Migrate students from old groups
            foreach (var oldGroup in groups)
            {
                var enrollments = await _enrollmentService.GetByScheduleIdAsync(oldGroup.Id);
                foreach (var enrollment in enrollments)
                {
                    var newEnrollment = new Enrollment
                    {
                        StudentId = enrollment.StudentId,
                        ScheduleId = createdGroup.Id,
                        EnrolledDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _enrollmentService.CreateAsync(newEnrollment);
                }
                
                // 5. Deactivate old group
                oldGroup.IsActive = false;
                await _scheduleService.UpdateAsync(oldGroup);
            }
        }
    }
}
```

### Scenario 4: Instructor Substitution
**Business Need**: Regular instructor is unavailable, need temporary substitution.

**Implementation**:
```csharp
public async Task SubstituteInstructorAsync(Guid groupId, Guid substituteInstructorId, DateTime effectiveFrom, DateTime? effectiveTo)
{
    var group = await _scheduleService.GetByIdAsync(groupId);
    if (group == null) throw new ArgumentException("Group not found");
    
    // Store original instructor for restoration
    var originalInstructorId = group.InstructorId;
    
    // Update group with substitute instructor
    group.InstructorId = substituteInstructorId;
    group.Name = $"{group.DanceClass.Name} {group.Level} - {substituteInstructorName}";
    group.UpdatedAt = DateTime.UtcNow;
    
    await _scheduleService.UpdateAsync(group);
    
    // Schedule restoration of original instructor
    if (effectiveTo.HasValue)
    {
        await ScheduleInstructorRestorationAsync(groupId, originalInstructorId, effectiveTo.Value);
    }
}
```

## Business Benefits

### 1. Natural Group Evolution
- Groups can progress through levels naturally
- Instructor changes are handled seamlessly
- Time and location changes are supported

### 2. Resource Optimization
- Small groups can be merged efficiently
- Instructor workload can be balanced
- Room utilization can be optimized

### 3. Student Experience
- Students stay with their group as it progresses
- Clear progression path from P1 to advanced levels
- Consistent scheduling with occasional adjustments

### 4. Administrative Efficiency
- Simple operations for common scenarios
- Clear audit trail of group changes
- Flexible semester planning

## Data Integrity

### Relationships
- **Schedule** → **DanceClass**: Each group belongs to a dance style
- **Schedule** → **User**: Each group has an assigned instructor
- **Schedule** → **Room**: Each group is held in a specific room
- **Enrollment** → **Schedule**: Students enroll in specific groups
- **Attendance** → **Schedule**: Attendance is tracked per group session

### Constraints
- Group level must be valid (P1, P2, P3, S1, S2, S3)
- Instructor must be available at the scheduled time
- Room must be available at the scheduled time
- Group capacity must not exceed room capacity

This architecture supports the dynamic nature of dance studio operations while maintaining data integrity and providing clear business logic for common scenarios. 