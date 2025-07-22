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

## Attendance Cancellation Logic (2025 Redesign)

### StudioScheduler Implementation

- **Explicit Cancellation Records**: Attendance cancellations are now tracked using explicit attendance records with an `IsCanceled` flag.
- **Student-Initiated Cancellation**: When a student cancels their attendance for a class, a record is created for that student and date with `IsCanceled = true`.
- **School-Initiated Cancellation**: When the school cancels a class, a record is created for that date with `IsCanceled = true` and `StudentId = null` (applies to all students).
- **Attendance Window Logic**: The system checks for both global (school-wide) and student-specific cancellation records when building the attendance window. If a global cancellation exists for a date, the class is considered canceled for all students. If a student-specific cancellation exists, only that student sees the class as canceled.
- **Testing & Compliance**: Unit tests cover both cancellation scenarios and edge cases, ensuring the logic aligns with business requirements and is robust against regressions.

---

## StudioScheduler Pass System Compliance (2024-07 Update)

### Migration to Pass-Based Enrollment Activity
- **Enrollment activity is now determined by Pass state**: Only students with an active, valid Pass (`Pass.IsActive`, `Pass.StartDate`, `Pass.EndDate`) are considered enrolled for attendance and eligibility.
- **Removed Enrollment.IsActive**: The `IsActive` property was removed from the Enrollment entity and database. All logic now checks Pass validity.
- **Business Logic Alignment**: This change ensures that only students with a valid Pass can attend classes, matching SalsaMe's business rules.
- **Data Integrity**: Orphaned or duplicate enrollments are eliminated, and all attendance/eligibility logic is centralized on Pass state.

### Compliance Summary
- **Pass Validity**: Fully matches SalsaMe's 28-day validity and active-pass requirement.
- **Attendance Eligibility**: Only students with a valid Pass can attend or be marked present, as in SalsaMe.
- **Make-up Classes, Freezing, Level Restrictions**: Not yet implemented in StudioScheduler (see `SOLUTION_STRUCTURE.md` for roadmap).
- **Pricing System**: Not yet implemented.

### Remaining Gaps
- **Make-up class tracking, pass freezing, and level restrictions** are not yet implemented in StudioScheduler, but are planned for future phases.

---

**Last reviewed:** 2024-07-21