# Studio Scheduler - Architecture & Development Guidelines

## Project Overview
Studio Scheduler is a cloud-hosted web application for managing dance/fitness studio classes, with both desktop (admin/operator) and mobile (student) interfaces.

## Technical Stack

### Frontend
- Blazor WebAssembly (.NET 9, upgrade to .NET 10 LTS planned)
- MudBlazor for UI components
- Fluxor for state management
- PWA capabilities for mobile

### Backend
- ASP.NET Core 9 API
- Entity Framework Core 9
- SignalR for real-time features
- JWT-based authentication (simple, role-based)

### Infrastructure
- Azure hosting (App Service)
- PostgreSQL on Azure
- Azure Application Insights
- Azure Blob Storage

## Solution Structure
```
StudioScheduler/
├── src/
│   ├── StudioScheduler.Client/           # Blazor WASM
│   ├── StudioScheduler.Server/           # ASP.NET Core API
│   ├── StudioScheduler.Core/             # Domain Models & Interfaces
│   ├── StudioScheduler.Infrastructure/    # Data Access & External Services
│   └── StudioScheduler.Shared/           # DTOs & Shared Models
├── tests/
│   ├── StudioScheduler.UnitTests/
│   └── StudioScheduler.IntegrationTests/
└── tools/                                # DB migrations, scripts
```

## Key Features

### Authentication
- Simple JWT-based authentication
- Basic user model with minimal fields:
  - Email, Name, Surname
  - Gender, Age
  - Role (Admin, DeskPerson, Student)

### Real-time Features (SignalR)
- Keep-alive functionality
- Class availability updates
- Spot opening notifications
- Instructor announcements
- Emergency notifications

### Core Functionality
- Class management
- User management
- Pass system management
- Reservation system
- Basic reporting
- Email notifications

## Database Schema
- Users (id, email, firstName, lastName, passwordHash, gender, dateOfBirth, role)
- Classes (id, name, description, capacity, instructorId)
- Schedule (id, classId, startTime, duration, isRecurring)
- Passes (id, userId, startDate, endDate, passType, classesPerWeek)
- Reservations (id, userId, scheduleId, passId, createdAt, status)

## Monitoring
- Application Insights integration
- Custom telemetry for business events
- Performance monitoring
- User behavior analytics
- Error tracking
- Custom dashboards for business metrics

## Development Guidelines

### Code Style
- Use C# 12 features where appropriate
- Follow Clean Architecture principles
- Implement SOLID principles
- Use async/await consistently
- Use inline namespaces for clarity
- Always remove unused usings
- Use meaningful variable and method names
- Keep methods short and focused
- Use dependency injection for services
- Avoid magic strings; use constants or enums
- Use FluentValidation for model validation
- Use AutoMapper for DTO mapping
- Use Serilog for structured logging
- Use MediatR for CQRS patterns
- Use FluentAssertions for unit tests
- Use xUnit for unit tests
- Use Moq for mocking in tests
- Use EF Core migrations for database schema changes
- Use Git for version control with clear commit messages
- Use feature branches for new features
- Use Markdown for documentation
- Use GitHub Actions for CI/CD workflows

### Performance Considerations
- Implement proper caching strategies
- Use SignalR with MessagePack protocol
- Monitor and optimize database queries
- Implement proper indexing strategy

### Security
- Always validate input
- Implement proper authorization
- Use HTTPS everywhere
- Secure sensitive configuration in Azure Key Vault
- Regular security updates

### Testing
- Unit tests for business logic
- Integration tests for API endpoints
- Load testing for critical paths
- End-to-end testing for critical flows

## Property Patterns

### Required String Properties

For required string properties in domain models, use the following pattern:

```csharp
public required string PropertyName { get; init; }
```

**Benefits:**
- Clean, concise syntax
- Immutable after object initialization (good for domain models)
- Compiler enforces initialization through `required` modifier
- No need for explicit null checking in setters
- Follows modern C# best practices

**Example:**
```csharp
public class Location
{
    public required string Name { get; init; }
    public required string Address { get; init; }
    public string Description { get; init; } = string.Empty;
}
```

**When to use:**
- Domain models where properties shouldn't change after creation
- Required properties that must be set during object initialization
- Properties that don't need complex validation logic

**When NOT to use:**
- Properties that need to be modified after object creation
- Properties requiring complex validation logic in setters
- DTOs or other objects that need mutable properties

### Optional String Properties

For optional string properties, provide a default value:

```csharp
public string Description { get; init; } = string.Empty;
```

This avoids nullable reference type warnings while maintaining the init-only pattern.

## Terminal Commands Guidelines

### Windows PowerShell Commands
When running commands in Windows PowerShell, be aware of these common issues:

**❌ WRONG - Don't use these patterns:**
```bash
# Unix-style command chaining (doesn't work in PowerShell)
cd tests/StudioScheduler.UnitTests && dotnet test

# Single ampersand (reserved for future use)
cd tests\StudioScheduler.UnitTests & dotnet test
```

**✅ CORRECT - Use these patterns instead:**
```powershell
# Use semicolon for command chaining
cd tests\StudioScheduler.UnitTests; dotnet test

# Or run commands separately
cd tests\StudioScheduler.UnitTests
dotnet test

# Or use full paths from current directory
dotnet test tests\StudioScheduler.UnitTests\StudioScheduler.UnitTests.csproj

# For building the entire solution
dotnet build

# For running specific test projects
dotnet test tests\StudioScheduler.UnitTests
dotnet test tests\StudioScheduler.IntegrationTests
```

**Key Rules:**
- Use backslashes `\` for Windows paths
- Use semicolons `;` for command chaining in PowerShell
- Prefer full paths when possible to avoid directory navigation issues
- Always test commands in a Windows environment before documenting

## Deployment Strategy
- CI/CD through Azure DevOps
- Automated testing in pipeline
- Staged deployments
- Zero-downtime updates when possible
- Regular database backups

## Implementation Notes

The init-only property pattern was implemented in the Location model to replace the previous custom getter/setter pattern that included explicit null checking. The `required` modifier ensures properties are initialized, eliminating the need for runtime null validation.

Remember: Keep It Simple! Start with core features and expand based on user feedback.
