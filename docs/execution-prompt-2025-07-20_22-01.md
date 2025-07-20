# Execution Prompt: Remove Schedule.Name Field

## Objective
Execute the task list in `docs/tasks-2025-07-20_22-01.md` systematically, working through each task one by one while following coding principles (KISS, YAGNI, SRP, DRY) and `.clinerules` guidelines.

## Instructions for Execution

### Task Execution Process
1. **Read the current task list** from `docs/tasks-2025-07-20_22-01.md`
2. **Work on each unchecked task** `[ ]` in sequential order
3. **Complete the task** using appropriate tools and following coding standards
4. **Mark the task as done** `[x]` in the task list file after completion
5. **Move to the next unchecked task** and repeat

### Coding Standards to Follow
- **KISS**: Keep solutions simple and understandable
- **YAGNI**: Don't add unnecessary functionality 
- **SRP**: Each component should have a single responsibility
- **DRY**: Avoid duplication only when it improves clarity
- **C# Standards**: Use C# 12 features, async/await, proper property patterns
- **Clean Architecture**: Follow established project structure
- **Windows PowerShell**: Use semicolon chaining, backslash paths

### Key Requirements
- **Examine existing code** before making changes to avoid duplication
- **Replace Schedule.Name with DanceClass.Name** in UI components
- **Create EF Core migrations** for database schema changes
- **Update tests** to reflect the changes
- **Use structured logging** with meaningful context
- **Follow project patterns** established in the codebase

### Database Considerations
- SQLite database location: `src/StudioScheduler.Server/studioscheduler.db`
- Use EF Core migrations for schema changes
- Update ApplicationDbContext and model snapshot

### Testing Requirements
- Run tests after significant changes: `dotnet test`
- Update unit tests, integration tests, and Playwright tests
- Ensure no breaking changes are introduced

### Documentation
- Document findings in analysis reports
- Create migration notes for database changes
- Update any relevant documentation files

## Current Task List Location
`docs/tasks-2025-07-20_22-01.md`

## How to Use This Prompt
Copy this prompt and use it to systematically work through removing the Schedule.Name field from the StudioScheduler solution while maintaining code quality and following established patterns.
