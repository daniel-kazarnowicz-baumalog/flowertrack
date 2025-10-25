# FLOWerTRACK Backend API

**Version:** 0.1.0-alpha  
**Framework:** .NET 10  
**Architecture:** Clean Architecture + CQRS + DDD

---

## 📋 Overview

FLOWerTRACK Backend is a comprehensive service ticketing system API built with .NET 10, implementing Clean Architecture, CQRS pattern, and Domain-Driven Design principles. It provides GraphQL and REST endpoints for managing service tickets, organizations, machines, and users.

### Key Features

- 🎯 **Clean Architecture** - Clear separation of concerns across layers
- 📊 **CQRS** - Command Query Responsibility Segregation with MediatR
- 🏗️ **DDD** - Rich domain models with business logic
- 🔐 **Supabase Auth** - JWT-based authentication
- 📁 **Supabase Storage** - File storage for attachments
- 🔍 **GraphQL API** - Flexible querying with HotChocolate
- 🚀 **REST API** - Traditional endpoints for machine logs
- ✅ **Comprehensive Testing** - Unit, integration, and API tests

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────┐
│              Flowertrack.Api                    │
│         (GraphQL + REST Controllers)            │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│         Flowertrack.Infrastructure              │
│      (External Concerns & Persistence)          │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│          Flowertrack.Application                │
│          (Use Cases & CQRS)                     │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│            Flowertrack.Domain                   │
│            (Core Business Logic)                │
└─────────────────────────────────────────────────┘
```

### Project Structure

```
src/backend/
├── Flowertrack.Domain/              # Core business logic
│   ├── Common/                      # Base classes & interfaces
│   ├── Entities/                    # Domain entities
│   ├── ValueObjects/                # Value objects
│   ├── Events/                      # Domain events
│   ├── Exceptions/                  # Domain exceptions
│   ├── Repositories/                # Repository interfaces
│   └── Services/                    # Domain services
│
├── Flowertrack.Application/         # Application layer
│   ├── Common/                      # Shared application logic
│   ├── Tickets/                     # Ticket use cases
│   ├── Organizations/               # Organization use cases
│   ├── Machines/                    # Machine use cases
│   ├── Users/                       # User use cases
│   └── Dashboard/                   # Dashboard queries
│
├── Flowertrack.Infrastructure/      # Infrastructure layer
│   ├── Persistence/                 # EF Core, repositories
│   ├── Identity/                    # Authentication
│   ├── Files/                       # File storage
│   ├── External/                    # External APIs
│   └── BackgroundJobs/              # Hangfire jobs
│
├── Flowertrack.Contracts/           # Shared DTOs
│   ├── Requests/                    # Request models
│   └── Responses/                   # Response models
│
└── Flowertrack.Api/                 # API layer
    ├── Controllers/                 # REST controllers
    ├── GraphQL/                     # GraphQL types & resolvers
    ├── Middleware/                  # Custom middleware
    └── Extensions/                  # Service extensions
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (RC2 or later)
- [Supabase Account](https://supabase.com) (for auth, database, storage)
- [Visual Studio 2025 Preview](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Quick Start

1. **Clone the repository**
   ```powershell
   git clone [repository-url]
   cd flowertrack-project/src/backend
   ```

2. **Configure User Secrets**
   ```powershell
   cd Flowertrack.Api
   dotnet user-secrets init
   dotnet user-secrets set "Supabase:Url" "https://[project-id].supabase.co"
   dotnet user-secrets set "Supabase:AnonKey" "[your-anon-key]"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "[your-connection-string]"
   ```

3. **Restore packages**
   ```powershell
   cd ..
   dotnet restore
   ```

4. **Build the solution**
   ```powershell
   dotnet build
   ```

5. **Run migrations**
   ```powershell
   cd Flowertrack.Infrastructure
   dotnet ef database update --startup-project ../Flowertrack.Api
   ```

6. **Run the application**
   ```powershell
   cd ../Flowertrack.Api
   dotnet run
   ```

7. **Access the API**
   - GraphQL Playground: https://localhost:5001/graphql
   - Health Check: https://localhost:5001/health
   - OpenAPI: https://localhost:5001/swagger (when configured)

---

## 🧪 Testing

### Run All Tests
```powershell
dotnet test
```

### Run Specific Test Project
```powershell
dotnet test Flowertrack.Domain.Tests/
```

### Run with Coverage
```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## 📦 NuGet Packages

### Core
- `MediatR` - CQRS implementation
- `FluentValidation` - Request validation
- `AutoMapper` - Object mapping
- `Ardalis.Specification` - Repository pattern
- `Ardalis.Result` - Result pattern

### GraphQL
- `HotChocolate.AspNetCore` - GraphQL server
- `HotChocolate.Data` - Filtering, sorting, paging

### Database
- `Microsoft.EntityFrameworkCore` - ORM
- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider
- `supabase-csharp` - Supabase SDK

### Authentication
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT auth

### Background Jobs
- `Hangfire.AspNetCore` - Background processing
- `Hangfire.PostgreSql` - Hangfire storage

### Logging
- `Serilog.AspNetCore` - Structured logging
- `Serilog.Sinks.Console` - Console output
- `Serilog.Sinks.File` - File logging

### Testing
- `xUnit` - Test framework
- `FluentAssertions` - Fluent assertions
- `Moq` - Mocking framework
- `Testcontainers.PostgreSql` - Integration testing

---

## 📚 Documentation

- [Implementation Tracker](../../.github/implementation/IMPLEMENTATION-TRACKER.md) - Overall progress
- [Phase 0: Setup](../../.github/implementation/PHASE-0-SETUP.md) - Infrastructure setup
- [Phase 1: Domain](../../.github/implementation/PHASE-1-DOMAIN.md) - Domain layer
- [Configuration Guide](../../.github/instructions/configuration.instructions.md) - Setup instructions
- [PRD](../../.ai/PRD.md) - Product requirements
- [Database Schema](../../.ai/db-plan.md) - Database design

---

## 🔐 Security

- **Authentication:** JWT tokens from Supabase Auth
- **Authorization:** Role-based access control (RBAC)
- **Data Isolation:** Row-Level Security (RLS) in PostgreSQL
- **API Keys:** Secure machine API tokens
- **HTTPS:** All endpoints require HTTPS in production

---

## 🌍 Environment Variables

### Required
- `Supabase:Url` - Supabase project URL
- `Supabase:AnonKey` - Supabase anonymous key
- `Supabase:ServiceKey` - Supabase service role key
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection string

### Optional
- `Supabase:JwtSecret` - JWT secret for validation
- `Logging:LogLevel:Default` - Logging level
- `Cors:AllowedOrigins` - CORS origins

---

## 🛠️ Development

### Code Style
- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use nullable reference types (`<Nullable>enable</Nullable>`)
- Use implicit usings where appropriate

### Commit Conventions
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `test:` - Test additions/changes
- `refactor:` - Code refactoring
- `chore:` - Maintenance tasks

### Branch Strategy
- `main` - Production-ready code
- `develop` - Integration branch
- `feature/*` - Feature branches
- `bugfix/*` - Bug fix branches

---

## 📊 Current Status

**Version:** 0.1.0-alpha  
**Build:** ✅ Passing  
**Tests:** ⚪ In Progress  
**Coverage:** TBD  
**Phase:** Phase 1 (Domain Layer)

### Completed
- ✅ Solution structure
- ✅ Domain base classes
- ✅ Basic value objects (enums)
- ✅ Project dependencies
- ✅ Documentation structure

### In Progress
- 🟡 Core domain entities
- 🟡 Value objects implementation
- 🟡 Domain events
- 🟡 Repository interfaces

### Upcoming
- ⚪ Application layer (CQRS)
- ⚪ Infrastructure layer (EF Core, Supabase)
- ⚪ GraphQL API
- ⚪ Authentication & Authorization
- ⚪ Background jobs

---

## 👥 Team

- **Product Owner:** [Name]
- **Tech Lead:** [Name]
- **Developers:** [Names]

---

## 📄 License

[License Type] - See LICENSE file for details

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

---

## 📞 Support

For questions or issues:
- Create an issue in GitHub
- Contact: [support email]
- Documentation: [link to docs]

---

**Built with ❤️ using .NET 10**
