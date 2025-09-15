---
goal: Refactor Error Handling and Logging for Controllers and Services
version: 1.0
date_created: 2025-09-15
last_updated: 2025-09-15
owner: StudioScheduler Team
status: 'Planned'
tags: [refactor, error-handling, logging, bug]
---

# Introduction

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This implementation plan aims to refactor error handling and logging in controllers and services to improve troubleshooting, provide actionable error messages, and ensure consistency across the solution.

## 1. Requirements & Constraints

- **REQ-001**: Use specific exception types instead of broad `Exception` where possible
- **REQ-002**: Provide actionable, user-friendly error messages in API responses
- **REQ-003**: Centralize error handling logic for maintainability
- **CON-001**: No breaking changes to public API endpoints
- **GUD-001**: Follow .NET best practices for error handling and logging
- **PAT-001**: Use structured logging with context

## 2. Implementation Steps

### Implementation Phase 1
- GOAL-001: Refactor error handling in PassController and AttendanceController

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-001 | Replace broad `catch (Exception)` with specific exception types in PassController | ✅ | 2025-09-15 |
| TASK-002 | Refactor error messages in PassController to be actionable and user-friendly | ✅ | 2025-09-15 |
| TASK-003 | Replace broad `catch (Exception)` with specific exception types in AttendanceController | ✅ | 2025-09-15 |
| TASK-004 | Refactor error messages in AttendanceController to be actionable and user-friendly | ✅ | 2025-09-15 |
| TASK-005 | Add structured logging with context in both controllers | ✅ | 2025-09-15 |

### Implementation Phase 2
- GOAL-002: Centralize error handling logic and update services

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-006 | Implement centralized error handling middleware (e.g., ExceptionFilter) | ✅ | 2025-09-15 |
| TASK-007 | Refactor service layer to throw specific exceptions | ✅ | 2025-09-15 |
| TASK-008 | Update service error messages for clarity and context | ✅ | 2025-09-15 |
| TASK-009 | Add structured logging to service layer | ✅ | 2025-09-15 |

## 3. Alternatives

- **ALT-001**: Use global exception handler only (not chosen due to need for granular control)
- **ALT-002**: Leave error handling as-is (not chosen due to poor troubleshooting and maintainability)

## 4. Dependencies

- **DEP-001**: Microsoft.Extensions.Logging
- **DEP-002**: ASP.NET Core ExceptionFilter or Middleware

## 5. Files

- **FILE-001**: src/StudioScheduler.Server/Controllers/PassController.cs
- **FILE-002**: src/StudioScheduler.Server/Controllers/AttendanceController.cs
- **FILE-003**: src/StudioScheduler.Infrastructure/Services/*
- **FILE-004**: src/StudioScheduler.Server/Middleware/* (if needed)

## 6. Testing

- **TEST-001**: Unit tests for controller error handling
- **TEST-002**: Integration tests for API error responses
- **TEST-003**: Logging verification tests

## 7. Risks & Assumptions

- **RISK-001**: Refactoring may introduce regressions if not thoroughly tested
- **ASSUMPTION-001**: Existing tests cover main error scenarios

## 8. Related Specifications / Further Reading

- [Microsoft Docs: Error Handling in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [Microsoft Docs: Logging in .NET](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging)
