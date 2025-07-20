# Migration Notes: Removal of Schedule.Name Field

**Date:** 2025-07-20  
**Migration:** RemoveScheduleName  
**Database:** SQLite (`src/StudioScheduler.Server/studioscheduler.db`)

## Summary

This migration removes the `Name` column from the `Schedules` table as part of the refactor to eliminate the `Schedule.Name` property from the domain model, DTOs, and all usages throughout the StudioScheduler solution.

## Migration Steps

1. **Model Update:**  
   - Removed `Name` property from `Schedule` class (`src/StudioScheduler.Core/Models/Schedule.cs`).

2. **DbContext Update:**  
   - Removed mapping for `Name` column in `ApplicationDbContext`.

3. **Migration Creation:**  
   - Created EF Core migration `RemoveScheduleName` to drop the `Name` column from the `Schedules` table.

4. **Migration Correction:**  
   - Edited migration to remove unrelated index/column drops for `Passes` table due to legacy migration issues.

5. **Database Update:**  
   - Applied migration successfully after correction.  
   - Verified that the `Schedules` table no longer contains the `Name` column.

## Impact

- All references to `Schedule.Name` have been removed from backend, API, frontend, and tests.
- UI components now use `DanceClass.Name` for display purposes.
- No data loss except for the removed `Name` column in `Schedules`.
- No breaking changes detected after running all tests.

## Troubleshooting

- If you encounter errors related to missing indexes (e.g., `IX_Passes_StudentId`), ensure migrations do not attempt to drop non-existent indexes.
- If database schema is out of sync, consider recreating the database or manually correcting migration history.

## Verification

- All tests pass (`dotnet test`).
- Database schema matches the updated model.
- No references to `Schedule.Name` remain in the solution.
