# Codebase Analysis Request

I need you to analyze the codebase located in the `src/StudioScheduler.Server` directory. Please focus particularly on identifying and understanding components named or related to `Client` and `Repository` patterns.

## Project Context
This analysis focuses on the Studio Scheduler application (.NET 9 Blazor WebAssembly + ASP.NET Core API solution).
Pay special attention to:
- Entity Framework patterns and data access layers
- Blazor component structure and state management (Fluxor)
- SignalR real-time features implementation
- Authentication and authorization patterns (JWT-based)
- MudBlazor component usage and patterns
- Domain model relationships (DanceClass, Schedule, User, Pass, etc.)

## Before Analysis - Planning Phase

1. **Create a preliminary audit plan** following the Planning and Documentation guidelines:
   - Record the current timestamp for documentation purposes
   - Create a draft plan in `docs/plan-{timestamp}.md`
   - Develop a detailed task list in `docs/tasks-{timestamp}.md` with checkboxes
   - Ensure all planning documents adhere to the required format

2. **Ask any clarifying questions** you need before finalizing the plan:
   - What specific issues or concerns should be prioritized? (Focus on: `performance and maintainability`)
   - Are there known problems with the existing implementation?
   - Which architectural patterns or coding standards should be evaluated against?
   - Are there specific performance or maintainability metrics to consider?
   - What is the intended purpose and usage context of components containing `Client` and `Repository`?
   - Are there any recent changes or problem areas that need special attention?

## Analysis Objectives
- [ ] Review overall architecture and solution structure
- [ ] Identify code quality issues and technical debt
- [ ] Evaluate adherence to established coding standards (.NET 9, C# 12 patterns)
- [ ] Assess component relationships and dependencies
- [ ] Review error handling and logging patterns (Serilog)
- [ ] Identify performance bottlenecks or concerns
- [ ] Evaluate test coverage and testing patterns (xUnit, FluentAssertions)
- [ ] Analyze Entity Framework usage and database patterns
- [ ] Review Blazor component lifecycle and state management
- [ ] Assess SignalR implementation and real-time features
- [ ] Evaluate authentication/authorization implementation
- [ ] Check adherence to required/init property patterns

## Analysis Scope
- **Time Investment**: Plan for thorough analysis without rushing
- **Depth Level**: Focus on architectural patterns and major issues first, then dive into implementation details
- **Priority Areas**: 
  1. Business logic and domain models
  2. Data access patterns and Entity Framework usage
  3. Blazor component structure and user interfaces
  4. API controllers and service layer
  5. Authentication and security patterns

## Expected Deliverables
1. **Architecture Overview** - High-level structure analysis with component relationships
2. **Component Analysis** - Detailed review of `Client` and `Repository` components
3. **Code Quality Assessment** - Issues categorized by severity and impact
4. **Technical Debt Inventory** - Prioritized list of problems with effort estimates
5. **Improvement Recommendations** - Specific, actionable refactoring suggestions
6. **Implementation Roadmap** - Phased approach for improvements aligned with KISS/YAGNI/SRP principles

## Evaluation Criteria
Assess the codebase against these Studio Scheduler specific patterns:
- **Property Patterns**: Use of `required string Property { get; init; }` for domain models
- **Clean Architecture**: Proper separation between Core, Infrastructure, and presentation layers
- **Entity Framework**: Repository patterns, migrations, and query optimization
- **Blazor Best Practices**: Component lifecycle, state management, and performance
- **SignalR Implementation**: Connection management, hub organization, and message handling
- **Testing Strategy**: Unit test coverage, integration test patterns, and test organization

## Report Structure
Please provide a comprehensive review with these sections:

### 1. Executive Summary
- Overall code health assessment
- Critical issues requiring immediate attention
- Compliance with established coding standards

### 2. Architecture Analysis
- Solution structure evaluation
- Layer separation and dependencies
- Design pattern usage assessment

### 3. Component Deep Dive
- Detailed analysis of `Client` components
- Detailed analysis of `Repository` components
- Interaction patterns and relationships

### 4. Issues & Recommendations
- **Critical Issues**: Security, performance, or stability problems
- **Major Issues**: Architecture violations, code quality concerns
- **Minor Issues**: Style inconsistencies, minor optimizations
- **Recommendations**: Specific improvement suggestions with rationale

### 5. Implementation Plan
- Prioritized improvement roadmap
- Effort estimates for major refactoring work
- Risk assessment for proposed changes

## Workflow Execution
Once you've developed the plan and task list, please:
1. Review them against the core principles (KISS, YAGNI, SRP, DRY) to ensure alignment
2. Present the plan and task list for my approval before proceeding with the full analysis
3. Only after receiving approval, execute the detailed code audit according to the approved plan

Include recommendations for refactoring opportunities that would improve code quality while maintaining adherence to KISS, YAGNI, and SRP principles. Focus on practical improvements that deliver real value without over-engineering.