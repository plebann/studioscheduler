# SalsaMe Business Model Analysis - Pass System

## Overview
Analysis of SalsaMe Dance Studio's actual business model based on their official price list, compared with our current StudioScheduler pass system documentation.

---

## SalsaMe Actual Pass System (June 2025)

### Monthly Passes (Valid for 28 days)
1. **1 course per week** (4 classes) - **130 PLN**
2. **2 courses per week** (8 classes) - **200 PLN**  
3. **3 courses per week** (12 classes) - **240 PLN**
4. **4 courses per week** (16 classes) - **280 PLN**
5. **5 courses per week** (20 classes) - **320 PLN**

### Monthly FLEXI Pass (Valid for 28 days)
1. **4 different classes** - **140 PLN**
2. **8 different classes** - **220 PLN**

### Other Options
1. **Single class** (1h) - **40 PLN** *(only for levels P2 and higher)*
2. **FULLPASS** (unlimited entry to all regular classes) - **350 PLN**

### Private Lessons (1 hour)
1. **Wedding first dance** - **180 PLN**
2. **Private lesson for 1 person** - **200 PLN**
3. **Private lesson for a couple** - **200 PLN**

---

## Business Rules & Policies

### Pass Validity & Usage
- **All passes valid for 28 days** (not calendar months)
- Make-up classes allowed within the 28-day validity period
- **Passes are NOT extended** under any circumstances
- Single entry NOT available for beginner courses (P1 level)

### Make-up Class Rules
- **1-2 courses per week**: Can make up 1 absence in another course (with prior notification)
- **3-5 courses per week**: Can make up 2 absences in another course (with prior notification)

### Pass Freezing Policy
- **Available once every 6 months**
- Must be frozen on **the day of first absence**
- **Maximum freeze period: 2 weeks**
- **Freeze costs:**
  - 4-entry pass (130 PLN) → 25 PLN
  - 8-entry pass (200 PLN) → 30 PLN
  - 12-entry pass (240 PLN) → 35 PLN
  - 16-entry pass (280 PLN) → 40 PLN
  - 20-entry pass (320 PLN) → 45 PLN
  - FULLPASS (350 PLN) → 50 PLN

---

## Comparison with Current StudioScheduler Pass System

### ✅ **What We Have Right**
1. **Pass Types**: Our `PassType` enum covers most scenarios:
   - `Weekly`, `Monthly`, `Quarterly`, `Annual`, `SingleClass`
2. **Core Model Structure**: 
   - `UserId`, `StartDate`, `EndDate`, `Type`, `ClassesPerWeek`, `RemainingClasses`
3. **Basic Functionality**: Active/inactive status, creation tracking

### ❌ **What We Need to Add/Modify**

#### 1. **Pass Pricing System**
```csharp
// Current: No pricing information
// Needed: Price storage and calculation
public decimal Price { get; set; }
public string Currency { get; set; } = "PLN";
```

#### 2. **28-Day Validity Model**
```csharp
// Current: Generic start/end dates
// Needed: Specific 28-day calculation
public DateTime CalculateEndDate(DateTime startDate) => startDate.AddDays(28);
```

#### 3. **FLEXI Pass Type**
```csharp
// Current: Only fixed weekly schedules
// Needed: Flexible class selection
public enum PassType
{
    Monthly,           // Fixed weekly schedule (1-5 courses per week)
    MonthlyFlexi,      // Flexible class selection (4 or 8 classes)
    SingleClass,
    FullPass,          // Unlimited access
    PrivateLesson
}
```

#### 4. **Make-up Class System**
```csharp
// Needed: Track allowed make-ups and usage
public int AllowedMakeupClasses { get; set; }
public int UsedMakeupClasses { get; set; }
public List<Guid> MakeupClassIds { get; set; } = new();
```

#### 5. **Pass Freezing System**
```csharp
// Needed: Freeze functionality
public bool IsFrozen { get; set; }
public DateTime? FreezeStartDate { get; set; }
public DateTime? FreezeEndDate { get; set; }
public int FreezeCount { get; set; } // Max 1 per 6 months
public decimal FreezeCost { get; set; }
```

#### 6. **Level Restrictions**
```csharp
// Needed: Single class restrictions for beginners
public List<string> RestrictedLevels { get; set; } = new(); // e.g., ["P1"] for single classes
```

---

## Recommended Pass System Updates

### 1. **New PassType Enum**
```csharp
public enum PassType
{
    Monthly1Course = 1,     // 1 course/week (4 classes) - 130 PLN
    Monthly2Courses = 2,    // 2 courses/week (8 classes) - 200 PLN
    Monthly3Courses = 3,    // 3 courses/week (12 classes) - 240 PLN
    Monthly4Courses = 4,    // 4 courses/week (16 classes) - 280 PLN
    Monthly5Courses = 5,    // 5 courses/week (20 classes) - 320 PLN
    MonthlyFlexi4 = 10,     // 4 flexible classes - 140 PLN
    MonthlyFlexi8 = 11,     // 8 flexible classes - 220 PLN
    SingleClass = 20,       // Single class - 40 PLN
    FullPass = 30,          // Unlimited - 350 PLN
    PrivateWedding = 40,    // Wedding lesson - 180 PLN
    PrivateSolo = 41,       // Private solo - 200 PLN
    PrivateCouple = 42      // Private couple - 200 PLN
}
```

### 2. **Enhanced Pass Model**
```csharp
public class Pass
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    
    // Core Pass Information
    public required PassType Type { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; } // Always StartDate + 28 days
    public required decimal Price { get; set; }
    public string Currency { get; set; } = "PLN";
    
    // Class Management
    public required int TotalClasses { get; set; }
    public required int RemainingClasses { get; set; }
    public required int ClassesPerWeek { get; set; } // 0 for flexible passes
    public bool IsFlexibleSchedule { get; set; } // true for FLEXI passes
    
    // Make-up Classes
    public int AllowedMakeupClasses { get; set; }
    public int UsedMakeupClasses { get; set; }
    public List<Guid> MakeupReservationIds { get; set; } = new();
    
    // Pass Freezing
    public bool IsFrozen { get; set; }
    public DateTime? FreezeStartDate { get; set; }
    public DateTime? FreezeEndDate { get; set; }
    public int FreezeCount { get; set; } // Max 1 per 6 months
    public decimal FreezeCost { get; set; }
    public DateTime? LastFreezeDate { get; set; }
    
    // Restrictions
    public List<string> RestrictedLevels { get; set; } = new();
    
    // Status
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<Reservation> Reservations { get; init; } = new List<Reservation>();
}
```

### 3. **Pass Configuration Service**
```csharp
public static class PassConfiguration
{
    public static readonly Dictionary<PassType, PassConfig> Configurations = new()
    {
        [PassType.Monthly1Course] = new(130m, 4, 1, 1, ["P1"]),
        [PassType.Monthly2Courses] = new(200m, 8, 2, 1, ["P1"]),
        [PassType.Monthly3Courses] = new(240m, 12, 3, 2, []),
        [PassType.Monthly4Courses] = new(280m, 16, 4, 2, []),
        [PassType.Monthly5Courses] = new(320m, 20, 5, 2, []),
        [PassType.MonthlyFlexi4] = new(140m, 4, 0, 1, []),
        [PassType.MonthlyFlexi8] = new(220m, 8, 0, 2, []),
        [PassType.SingleClass] = new(40m, 1, 0, 0, ["P1"]),
        [PassType.FullPass] = new(350m, int.MaxValue, 0, 0, []),
    };
}

public record PassConfig(
    decimal Price,
    int TotalClasses,
    int ClassesPerWeek,
    int AllowedMakeups,
    List<string> RestrictedLevels
);
```

---

## Business Impact Analysis

### 1. **Revenue Model**
- **Primary revenue**: Monthly passes (130-350 PLN)
- **Premium pricing**: Private lessons (180-200 PLN/hour)
- **Flexible options**: FLEXI passes for irregular attendance
- **Additional revenue**: Pass freezing fees (25-50 PLN)

### 2. **Customer Segmentation**
- **Beginners**: Forced into monthly passes (no single classes for P1)
- **Regular students**: 1-5 courses per week options
- **Casual students**: FLEXI passes for irregular attendance
- **Advanced students**: Single class options available
- **Special events**: Private wedding lessons

### 3. **Retention Strategies**
- **28-day validity**: Encourages regular attendance
- **Make-up classes**: Customer retention through flexibility
- **Pass freezing**: Prevents complete loss during absences
- **Level progression**: Single classes unavailable for beginners

---

## Implementation Priority

### Phase 1: Core Pass System
1. Update PassType enum with SalsaMe types
2. Enhance Pass model with pricing and 28-day validity
3. Implement pass configuration system
4. Update PassDto classes

### Phase 2: Make-up Classes
1. Add make-up tracking to Pass model
2. Implement make-up reservation logic
3. Add make-up validation rules
4. Update UI for make-up management

### Phase 3: Pass Freezing
1. Add freezing functionality to Pass model
2. Implement freeze cost calculation
3. Add freeze validation (once per 6 months)
4. Update UI for freeze management

### Phase 4: Advanced Features
1. Level restriction enforcement
2. Automated pass expiration
3. Revenue reporting
4. Customer analytics

---

*Analysis Date: 2025-06-15*
*Source: https://salsame.pl/en/price-list/*
*Purpose: Align StudioScheduler with real-world dance studio business model*
