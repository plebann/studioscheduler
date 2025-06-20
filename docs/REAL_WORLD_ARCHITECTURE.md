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

### Constraints
- Group level must be valid (P1, P2, P3, S1, S2, S3)
- Instructor must be available at the scheduled time
- Room must be available at the scheduled time
- Group capacity must not exceed room capacity

This architecture supports the dynamic nature of dance studio operations while maintaining data integrity and providing clear business logic for common scenarios. 