# Real-World Architecture: Dance Studio Business Model

## Overview
Studio Scheduler is designed around real-world dance studio operations, where the business model reflects actual teaching practices and group dynamics.

## Core Architecture

### DanceClass (Dance Style Definition)
**Purpose**: Defines dance styles/types that the studio offers
- **Static entities**: "Bachata", "Salsa Cubana", "Salsa on1", "Zouk", "Kizomba"
- **Style categorization**: BACHATA, SALSA, ZOUK, KIZOMBA, etc.
- **Reusable templates**: Multiple class groups can use the same dance style

### Schedule (Class Group)
**Purpose**: Represents actual class groups with specific characteristics
- **Dynamic entities**: Groups evolve over time
- **Level progression**: P1 → P2 → P3 → S1 → S2 → S3
- **Instructor assignment**: Can change due to availability, substitution, etc.
- **Time and location**: Specific weekly schedule in a particular room

### Pass System and Enrollment Validity (2024-07 Update)
**Purpose**: Models real-world access control and eligibility for class participation
- **Enrollment activity is now determined by Pass state**: Only students with an active, valid Pass (`Pass.IsActive`, `Pass.StartDate`, `Pass.EndDate`) are considered enrolled and eligible to attend classes.
- **Removed Enrollment.IsActive**: The `IsActive` property was removed from the Enrollment entity and database. All logic now checks Pass validity.
- **Business Alignment**: This change ensures that only students with a valid Pass can attend classes, matching SalsaMe's business rules and improving data integrity.
- **See also**: `SALSAME_BUSINESS_ANALYSIS.md` and `SOLUTION_STRUCTURE.md` for compliance and implementation details.

## Real-World Business Scenarios

### Scenario 1: Adding New Bachata P1 Classes
**Business Need**: Studio wants to offer more beginner Bachata classes due to high demand.

### Scenario 2: Class Group Progression
**Business Need**: After 3 months, the Bachata P1 group has progressed and should become P2.

### Scenario 3: Group Merging During Semester Reorganization
**Business Need**: At the end of semester, small groups need to be merged to optimize resources.

### Scenario 4: Instructor Substitution
**Business Need**: Regular instructor is unavailable, need temporary substitution.

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
- **Enrollment** → **Pass**: Enrollment validity is now determined by Pass state (2024-07)

### Constraints
- Group level must be valid (P1, P2, P3, S1, S2, S3)
- Instructor must be available at the scheduled time
- Room must be available at the scheduled time
- Group capacity must not exceed room capacity

This architecture supports the dynamic nature of dance studio operations while maintaining data integrity and providing clear business logic for common scenarios.