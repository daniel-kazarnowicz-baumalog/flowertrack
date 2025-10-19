# GitHub Copilot Instructions for FLOWerTRACK

> **Important:** These instructions apply to GitHub Copilot coding agent, Copilot Chat, and Copilot code review. Please read carefully and follow these guidelines when working on this repository.

## Project Overview

FLOWerTRACK is a comprehensive service ticket management system designed for companies servicing production equipment (such as Baumalog) and their clients. The platform centralizes the entire service workflow from issue reporting through resolution, providing transparency and efficiency for both service teams and customers.

**Current Status:** Early development phase - MVP in progress

### Tech Stack

**Backend:**
- .NET 10.0 (RC2)
- ASP.NET Core Web API
- C# with nullable reference types enabled
- Implicit usings enabled

**Frontend:**
- React 19.2 with TypeScript 5.9
- Vite build tool (using Rolldown variant 7.1.14)
- ESLint + Prettier for code quality
- React Hooks architecture (functional components only)

## Repository Structure

```
flowertrack-project/
├── src/
│   ├── backend/
│   │   └── Flowertrack.Api/          # ASP.NET Core Web API
│   │       ├── Program.cs             # Application entry point
│   │       ├── Flowertrack.Api.csproj # Project file
│   │       ├── appsettings.json       # Configuration
│   │       └── Properties/
│   │           └── launchSettings.json
│   └── frontend/
│       └── flowertrack-client/        # React + Vite application
│           ├── src/                   # Source files
│           ├── public/                # Static assets
│           ├── package.json           # Dependencies
│           ├── tsconfig.json          # TypeScript config
│           ├── vite.config.ts         # Vite configuration
│           └── eslint.config.js       # ESLint config
├── .github/
│   ├── copilot-instructions.md        # This file
│   ├── copilot-setup-steps.yml        # Environment setup
│   └── instructions/                  # Path-specific instructions
└── README.md                          # Project documentation
```

## Development Workflow

### Required Before Each Commit

**Backend:**
- Ensure code compiles without warnings: `dotnet build`
- Run code formatting: `dotnet format` (if configured)
- Follow C# coding conventions (PascalCase for classes/methods, camelCase for variables)

**Frontend:**
- Run linter: `npm run lint` or `npm run lint:fix` for auto-fixes
- Check formatting: `npm run format:check` or `npm run format` to fix
- Ensure TypeScript compiles: `npm run build`
- Maximum ESLint warnings: **0** (enforced)

### Build & Run Commands

**Backend (.NET API):**
```powershell
cd src/backend/Flowertrack.Api
dotnet restore          # Restore dependencies
dotnet build            # Build the project
dotnet run              # Run the API (default: https://localhost:5001)
```

**Frontend (React + Vite):**
```powershell
cd src/frontend/flowertrack-client
npm install             # Install dependencies
npm run dev             # Start development server (default: http://localhost:5173)
npm run build           # Production build
npm run preview         # Preview production build
```

### Testing Strategy

- Write unit tests for all new business logic
- Backend: Use xUnit or NUnit (to be determined)
- Frontend: Use Vitest or Jest with React Testing Library (to be configured)
- Maintain test coverage above 70% for critical paths

## Code Standards & Best Practices

### General Guidelines

1. **Trust these instructions first** - Only search for additional context if these instructions are incomplete or incorrect
2. **Maintain consistency** - Follow existing code patterns and architectural decisions
3. **Document complex logic** - Add XML comments (C#) or JSDoc (TypeScript) for non-trivial code
4. **Keep it simple** - Prefer readable code over clever code
5. **Security first** - Never commit sensitive data (API keys, passwords, connection strings)

### Backend (.NET/C#) Specific

1. **Use Dependency Injection** - Register services in `Program.cs`, inject via constructors
2. **Async/await everywhere** - All I/O operations should be asynchronous
3. **Nullable reference types** - Handle null cases explicitly (enabled in project)
4. **RESTful conventions** - Follow REST principles for API endpoints
5. **Naming conventions:**
   - PascalCase: Classes, Methods, Properties, Public fields
   - camelCase: Parameters, Local variables, Private fields (with `_` prefix)
   - UPPER_CASE: Constants
6. **Exception handling** - Use specific exception types, avoid catching generic `Exception`
7. **LINQ usage** - Prefer LINQ for collection operations
8. **Repository pattern** - Use repositories for data access (when implemented)

### Frontend (React/TypeScript) Specific

1. **Functional components only** - No class components
2. **TypeScript strict mode** - Always type props, state, and function parameters
3. **React Hooks:**
   - `useState` for local state
   - `useEffect` for side effects (cleanup when needed)
   - `useCallback` for memoized callbacks
   - `useMemo` for expensive computations
4. **Component structure:**
   - Keep components small and focused (< 200 lines)
   - Extract reusable logic into custom hooks
   - Co-locate styles with components
5. **Props naming:**
   - Use `onEventName` for event handlers
   - Use `isCondition` or `hasProperty` for booleans
6. **Accessibility:**
   - Include ARIA labels where needed
   - Ensure keyboard navigation works
   - Use semantic HTML elements
7. **Performance:**
   - Avoid unnecessary re-renders
   - Use `React.memo()` for expensive components
   - Lazy load routes/components when appropriate

### CSS/Styling

- Use modular CSS (CSS Modules or similar)
- Follow BEM naming convention for class names
- Ensure responsive design (mobile-first approach)
- Maintain consistent spacing and color schemes

## Project-Specific Context

### User Roles
1. **Service Technician** - Handles and resolves tickets
2. **Service Manager** - Oversees team and ticket distribution
3. **Client User** - Reports issues and tracks their tickets
4. **Client Admin** - Manages team members and permissions

### MVP Features (In Scope)
- User authentication and role-based access
- Ticket creation, assignment, and status tracking
- Basic notification system
- Machine/equipment registry
- Service history logging
- Simple reporting dashboard

### Excluded from MVP
- Advanced analytics and BI dashboards
- Mobile native applications
- Third-party integrations (Slack, Teams, etc.)
- Automated ticket routing AI
- Real-time chat functionality
- Multi-language support

## Common Pitfalls to Avoid

1. ❌ **Don't create duplicate code** - Check for existing utilities/helpers first
2. ❌ **Don't bypass validation** - Always validate user input on both client and server
3. ❌ **Don't ignore errors** - Handle errors gracefully with user-friendly messages
4. ❌ **Don't hardcode values** - Use configuration files for environment-specific settings
5. ❌ **Don't commit commented-out code** - Remove it or explain why it's needed
6. ❌ **Don't mix concerns** - Separate business logic from presentation logic
7. ❌ **Don't skip PR descriptions** - Explain what changed and why

## When Making Changes

### Before Starting
1. Read the related issue/ticket carefully
2. Check `README.md` for project context
3. Review existing similar code for patterns
4. Identify which files need changes

### During Development
1. Make small, focused commits with clear messages
2. Test changes locally before pushing
3. Ensure all linters pass
4. Update documentation if API/behavior changes

### Before Submitting PR
1. Review your own diff for obvious issues
2. Ensure CI checks will pass (build, lint, format)
3. Add tests for new functionality
4. Update relevant documentation
5. Write a clear PR description with:
   - What changed
   - Why it changed
   - How to test it
   - Screenshots (if UI changes)

## Error Handling & Debugging

**If build fails:**
- Backend: Check `dotnet build` output for errors, verify NuGet packages restored
- Frontend: Check `npm install` completed, verify Node.js version compatibility

**If tests fail:**
- Read test output carefully
- Run single test to isolate issue
- Check for race conditions in async tests
- Verify test data setup/teardown

**If linter fails:**
- Run `npm run lint:fix` (frontend) to auto-fix
- Review remaining errors manually
- Don't disable linter rules without discussion

## Additional Resources

- Project README: `/README.md`
- Backend project: `/src/backend/Flowertrack.Api/`
- Frontend project: `/src/frontend/flowertrack-client/`
- Path-specific instructions: `/.github/instructions/`

---

**Note to Copilot:** When uncertain about implementation details, prioritize code quality and maintainability over speed. Ask for clarification rather than making assumptions that could break existing functionality.
