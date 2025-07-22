# Task List – Fix Class Date Calculation and "Today" Logic in Attendance Modal
**Plan Timestamp:** 2025-07-21_XX-XX

## Tasks

1. [x] **Analyze and Refactor Class Date Calculation**
   - Review the logic in `AttendanceController.GetClassSchedule` that generates the 4-week attendance window.
   - Ensure the calculation always aligns each week’s date to the scheduled class day (e.g., Monday), not just relative to `DateTime.Today`.
   - Prevent duplicate dates in the attendance window.

2. [x] **Correct "Today" and "Present" Logic**
   - Update logic to only show the "Present" flag and checkbox if the current date matches the scheduled class day.
   - Ensure the "today" label and attendance marking are only available for the correct weekday.

3. [x] **Update Week Labeling**
   - Ensure week labels ("Last wk", "2wk ago", etc.) are mapped to the correct calendar weeks for the class schedule.

4. [x] **Add/Update Unit Tests**
   - Add or update tests to verify correct week calculation, date alignment, and "today" logic for various class days and current dates.

5. [x] **Build and Error Check**
   - Check if the solution builds successfully after changes.
   - Fix any compilation errors or warnings that arise.

6. [ ] **Manual Verification**
   - Manually verify the modal for different class days and current dates to ensure correct display and logic.

7. [ ] **Documentation**
   - Update documentation to describe the correct logic for class date calculation and attendance marking.

---
**All tasks must be marked as completed [x] upon finishing.**
