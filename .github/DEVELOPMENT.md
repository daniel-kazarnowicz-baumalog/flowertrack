# FLOWerTRACK - Development Cheat Sheet

Quick reference for common development tasks.

## Project Structure

```
FLOWerTRACK/
├── Backend (.NET 10.0)     → src/backend/Flowertrack.Api/
└── Frontend (React 19.2)   → src/frontend/flowertrack-client/
```

## Common Commands

### Backend (.NET)
```powershell
# Navigate to backend
cd src/backend/Flowertrack.Api

# Restore packages
dotnet restore

# Build
dotnet build

# Run (Development)
dotnet run

# Watch mode (auto-reload)
dotnet watch run

# Clean build
dotnet clean
dotnet build
```

### Frontend (React + Vite)
```powershell
# Navigate to frontend
cd src/frontend/flowertrack-client

# Install dependencies
npm install

# Start dev server (http://localhost:5173)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint

# Fix linting issues
npm run lint:fix

# Check formatting
npm run format:check

# Fix formatting
npm run format
```

## Development URLs

- **Backend API**: https://localhost:5001 (or http://localhost:5000)
- **Frontend**: http://localhost:5173
- **API Docs**: https://localhost:5001/swagger (when configured)

## Code Quality Checks

### Before Committing
```powershell
# Backend
cd src/backend
dotnet build

# Frontend
cd src/frontend/flowertrack-client
npm run lint
npm run format:check
npm run build
```

### Fix Common Issues
```powershell
# Frontend - Auto-fix linting
npm run lint:fix

# Frontend - Auto-fix formatting
npm run format
```

## Tech Stack Quick Reference

### Backend
- Framework: .NET 10.0
- Language: C# (nullable enabled)
- API Style: RESTful
- DI: Built-in ASP.NET Core

### Frontend
- Framework: React 19.2
- Language: TypeScript 5.9
- Build Tool: Vite (Rolldown)
- State: React Hooks
- Styling: CSS (modular)

## Naming Conventions

### C# (.NET)
- Classes/Methods: `PascalCase`
- Private fields: `_camelCase`
- Interfaces: `IPascalCase`
- Constants: `PascalCase` or `UPPER_CASE`

### TypeScript/React
- Components: `PascalCase`
- Functions/Variables: `camelCase`
- Constants: `UPPER_CASE`
- Files: `ComponentName.tsx`, `utilityName.ts`

## File Creation Patterns

### Backend - New API Controller
```csharp
// src/backend/Flowertrack.Api/Controllers/ExampleController.cs
[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _service;
    
    public ExampleController(IExampleService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExampleDto>>> Get()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }
}
```

### Frontend - New Component
```tsx
// src/frontend/flowertrack-client/src/components/ExampleComponent.tsx
import React from 'react';
import './ExampleComponent.css';

interface ExampleComponentProps {
  title: string;
  onAction?: () => void;
}

export const ExampleComponent: React.FC<ExampleComponentProps> = ({ 
  title, 
  onAction 
}) => {
  return (
    <div className="example-component">
      <h2>{title}</h2>
    </div>
  );
};
```

## Troubleshooting

### Backend won't start
1. Check .NET SDK version: `dotnet --version` (should be 10.0)
2. Restore packages: `dotnet restore`
3. Check port availability (5000/5001)

### Frontend won't start
1. Check Node.js version: `node --version` (18.x or higher)
2. Delete `node_modules` and reinstall: `rm -rf node_modules && npm install`
3. Check port 5173 is available

### Linting errors
1. Run auto-fix: `npm run lint:fix`
2. Check ESLint config: `eslint.config.js`
3. Review error messages - some may require manual fixes

## Git Workflow

```bash
# Create feature branch
git checkout -b feature/description

# Make changes and commit
git add .
git commit -m "feat: description of changes"

# Push and create PR
git push origin feature/description
```

## Commit Message Convention

- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `style:` - Code style changes (formatting)
- `refactor:` - Code refactoring
- `test:` - Adding tests
- `chore:` - Maintenance tasks

## Need Help?

1. Check `.github/copilot-instructions.md` for detailed guidelines
2. Review `.github/instructions/` for language-specific rules
3. Ask GitHub Copilot Chat with `@workspace` context
4. Check project README: `/README.md`
