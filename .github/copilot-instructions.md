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

## Deployment Strategy
- CI/CD through Azure DevOps
- Automated testing in pipeline
- Staged deployments
- Zero-downtime updates when possible
- Regular database backups

Remember: Keep It Simple! Start with core features and expand based on user feedback.
