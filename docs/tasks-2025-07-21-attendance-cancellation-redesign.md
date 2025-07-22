# Task List – Attendance Cancellation Logic Redesign
**Plan Timestamp:** 2025-07-21_StudentAndSchoolCancellation

## Tasks

[x] 1. Review and update the Attendance data model:
    - Ensure there is an `IsCanceled` (or `IsCancelled`) boolean property on attendance records.
    - Add a nullable `StudentId` property to attendance records to support global (school-wide) cancellations.

[x] 2. Update backend logic for attendance creation:
    - When a student cancels their attendance for a class, create a new attendance record for that student and date with `IsCanceled = true`.
    - When the school cancels a class, create a new attendance record for the class date with `IsCanceled = true` and `StudentId = null`.

[x] 3. Update attendance query logic:
    - When building the attendance window for a student, treat the class as canceled for all students if a global cancellation record exists for that date.
    - If a student-specific cancellation record exists, treat the class as canceled for that student for that date.
    - Otherwise, use normal attendance logic.

[x] 4. Update and add unit/integration tests:
    - Test student-initiated cancellation: only that student sees the class as canceled.
    - Test school-initiated cancellation: all students see the class as canceled.
    - Test normal attendance and edge cases (e.g., both student and school cancellation for the same date).

[x] 5. Update documentation:
    - The attendance cancellation logic now uses explicit attendance records with an `IsCanceled` flag and a nullable `StudentId` for global (school-wide) cancellations.
    - When a student cancels, a record is created for that student/date with `IsCanceled = true`.
    - When the school cancels, a record is created for that date with `IsCanceled = true` and `StudentId = null`.
    - The attendance window logic checks for global and student-specific cancellations.
    - Unit tests cover both cancellation scenarios and edge cases.

[ ] 6. Create and apply database migration:
    - Add the `IsCanceled` column and make `StudentId` nullable in the `Attendance` table.
    - Apply the migration to the development database.

---
**All tasks must be marked as completed [x] upon finishing.**
