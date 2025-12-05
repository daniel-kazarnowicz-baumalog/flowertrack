# FLOWerTRACK

> Advanced service ticket management system for production equipment maintenance

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19.2-61DAFB?logo=react)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Environment Variables](#environment-variables)
  - [Running the Application](#running-the-application)
- [Available Scripts](#available-scripts)
- [Project Structure](#project-structure)
- [Project Scope](#project-scope)
  - [MVP Features](#mvp-features)
  - [Excluded from MVP](#excluded-from-mvp)
  - [Technical Limitations](#technical-limitations)
- [User Roles](#user-roles)
- [Project Status](#project-status)
- [Contributing](#contributing)
- [License](#license)

## Overview

FLOWerTRACK is a comprehensive service ticket management system designed for companies servicing production equipment (such as Baumalog) and their clients. The platform centralizes the entire service workflow from issue reporting through resolution, providing transparency and efficiency for both service teams and customers.

### Problems We Solve

**For Service Teams:**
- Centralized ticket management from multiple sources (email, phone, HMI, machine logs)
- Real-time visibility into team workload and ticket status
- Complete audit trail of all actions and changes
- Quick access to technical context and machine history

**For Clients:**
- Direct access to machine status and service history
- Transparent communication with service teams
- Self-service team management capabilities
- Easy access to machine logs and documentation

## Key Features

- **Dual Portal Architecture**: Separate interfaces for service teams and clients
- **Automated Machine Log Integration**: Production machines automatically send error logs to the system
- **Complete Ticket Lifecycle Management**: From draft to closed with full workflow support
- **Timeline & Audit Trail**: Complete history of all actions, status changes, and communications
- **Role-Based Access Control**: Four distinct user roles with appropriate permissions
- **Advanced Filtering & Search**: Quick access to relevant tickets and information
- **File Attachments**: Support for images, documents, and videos (up to 10MB)
- **KPI Dashboards**: Real-time insights into service performance and workload
- **Internal Notes**: Private team communication separate from client-visible messages

## Tech Stack

### Frontend
- **Framework**: React 19.2 with TypeScript 5.9
- **Build Tool**: Vite (Rolldown)
- **UI Library**: DevExtreme (advanced data grids, charts, and components)
- **Styling**: Tailwind CSS
- **Code Quality**: ESLint, Prettier

### Backend
- **Framework**: .NET 10.0 (ASP.NET Core Web API)
- **ORM**: Entity Framework Core (Code-First approach)
- **API Documentation**: OpenAPI/Swagger

### Database
- **Primary Database**: PostgreSQL

### Infrastructure & DevOps
- **Frontend Hosting**: Vercel (global CDN deployment)
- **Backend Hosting**: Railway (containerized deployment)
- **Containerization**: Docker
- **CI/CD**: Automated via GitHub integration

## Getting Started

### Prerequisites

Ensure you have the following installed:

- **Node.js**: v20.x or higher (LTS recommended)
- **npm**: v9.x or higher
- **.NET SDK**: 10.0 or higher
- **PostgreSQL**: 14.x or higher
- **Docker**: (optional, for containerized development)
- **Git**: Latest version

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/daniel-kazarnowicz-baumalog/flowertrack.git
   cd flowertrack
   ```

2. **Install frontend dependencies**
   ```bash
   cd src/frontend/flowertrack-client
   npm install
   ```

3. **Restore backend dependencies**
   ```bash
   cd ../../backend/Flowertrack.Api
   dotnet restore
   ```

4. **Setup the database**
   ```bash
   # Create a PostgreSQL database
   createdb flowertrack_dev
   
   # Apply migrations (from backend directory)
   dotnet ef database update
   ```

### Environment Variables

Create environment configuration files:

**Frontend** (`src/frontend/flowertrack-client/.env.local`):
```env
VITE_API_URL=https://localhost:5001/api
VITE_APP_NAME=FLOWerTRACK
```

**Backend - Using User Secrets (Recommended for Development)**:

The recommended approach for local development is to use .NET User Secrets to store sensitive configuration:

```bash
# Navigate to the API project
cd src/backend/Flowertrack.Api

# Initialize user secrets
dotnet user-secrets init

# Set Supabase configuration
dotnet user-secrets set "Supabase:Url" "https://your-project.supabase.co"
dotnet user-secrets set "Supabase:AnonKey" "your-anon-key"
dotnet user-secrets set "Supabase:ServiceKey" "your-service-role-key"
dotnet user-secrets set "Supabase:JwtSecret" "your-jwt-secret"

# Set database connection
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.your-project.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-password"

# Verify secrets are set
dotnet user-secrets list
```

**Backend - Alternative: appsettings.Development.json (Not Recommended for Secrets)**:

⚠️ **Warning**: Never commit actual secrets to appsettings files. This is shown for reference only.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=xxx"
  },
  "Database": {
    "CommandTimeout": 30,
    "EnableSensitiveDataLogging": false
  },
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "AnonKey": "",
    "ServiceKey": "",
    "JwtSecret": ""
  }
}
```

📚 **For detailed Supabase setup instructions**, see [docs/SUPABASE-SETUP.md](docs/SUPABASE-SETUP.md)

### Running the Application

**Development Mode:**

1. **Start the backend API**
   ```bash
   cd src/backend/Flowertrack.Api
   dotnet run
   ```
   API will be available at `https://localhost:5001`

2. **Start the frontend (in a new terminal)**
   ```bash
   cd src/frontend/flowertrack-client
   npm run dev
   ```
   Frontend will be available at `http://localhost:5173`

**Using Docker** (coming soon):
```bash
docker-compose up
```

## Available Scripts

### Frontend Scripts

From `src/frontend/flowertrack-client`:

| Command | Description |
|---------|-------------|
| `npm run dev` | Start development server with hot reload |
| `npm run build` | Build production-ready application |
| `npm run preview` | Preview production build locally |
| `npm run lint` | Run ESLint to check code quality |
| `npm run lint:fix` | Auto-fix ESLint issues |
| `npm run format` | Format code with Prettier |
| `npm run format:check` | Check code formatting |

### Backend Scripts

From `src/backend/Flowertrack.Api`:

| Command | Description |
|---------|-------------|
| `dotnet run` | Start the API in development mode |
| `dotnet build` | Compile the project |
| `dotnet test` | Run unit tests |
| `dotnet ef migrations add <Name>` | Create a new database migration |
| `dotnet ef database update` | Apply pending migrations |
| `dotnet ef migrations remove` | Remove the last migration |

## Project Structure

```
flowertrack-project/
├── .ai/                          # AI-assisted documentation
│   ├── PRD.md                    # Product Requirements Document
│   └── Tech-stack.md             # Technology stack analysis
├── src/
│   ├── frontend/
│   │   └── flowertrack-client/   # React + TypeScript client
│   │       ├── src/
│   │       │   ├── components/   # Reusable UI components
│   │       │   ├── pages/        # Page components
│   │       │   ├── services/     # API communication layer
│   │       │   ├── hooks/        # Custom React hooks
│   │       │   ├── utils/        # Utility functions
│   │       │   └── types/        # TypeScript type definitions
│   │       └── public/           # Static assets
│   └── backend/
│       └── Flowertrack.Api/      # .NET Web API
│           ├── Controllers/      # API endpoints
│           ├── Models/           # Domain models
│           ├── Data/             # EF Core DbContext
│           ├── Services/         # Business logic
│           └── Migrations/       # Database migrations
└── README.md
```

## Project Scope

### MVP Features

#### Service Portal
- ✅ User authentication with role-based access
- ✅ Dashboard with KPI metrics and trend charts
- ✅ Advanced ticket list with filtering, sorting, and search
- ✅ Detailed ticket view with timeline/audit trail
- ✅ Ticket lifecycle management (Draft → Closed)
- ✅ Internal notes (invisible to clients)
- ✅ Public comments for client communication
- ✅ File attachment support (max 10MB)
- ✅ Ticket assignment to technicians
- ✅ Bulk actions on multiple tickets
- ✅ Organization management and onboarding
- ✅ Machine monitoring and status tracking
- ✅ API token generation for machine integration
- ✅ Service team administration

#### Client Portal
- ✅ Account activation via email link
- ✅ Dashboard with machine and ticket status
- ✅ Ticket creation and management
- ✅ Timeline view of ticket history
- ✅ Comment and attachment capabilities
- ✅ Ticket reopening (within 14 days after resolution)
- ✅ Team management (for Organization Admins)
- ✅ Operator invitation and access control

#### System Features
- ✅ Complete audit trail for all actions
- ✅ Machine log integration via API
- ✅ Status change workflow with mandatory justifications
- ✅ Export ticket history (PDF, CSV, JSON)
- ✅ Multi-language support preparation (Polish initially)
- ✅ Responsive design for mobile devices

### Excluded from MVP

The following features are **not** included in the initial MVP release:

- ❌ Real-time push notifications (email only for onboarding/login)
- ❌ Real-time chat (limited to asynchronous comments)
- ❌ Automatic ticket assignment algorithms
- ❌ External ticketing system integrations
- ❌ Advanced reporting and analytics (beyond basic KPIs)
- ❌ Automatic overdue ticket reminders
- ❌ Video call or remote support capabilities
- ❌ Multi-level routing to specialists
- ❌ Ticket templates
- ❌ SLA management and contract tracking
- ❌ Automatic ticket closure
- ❌ CRM system integration
- ❌ Billing and invoicing
- ❌ Regional/entity subdivision
- ❌ SAML/OAuth authentication

### Technical Limitations

- **Concurrent Users**: Maximum 100 simultaneous users in MVP
- **Log History**: Machine logs retained for 90 days
- **File Size**: Maximum 10MB per attachment
- **Supported File Types**: JPG, PNG, PDF, TXT, DOC, DOCX, XLS, XLSX, ZIP, MP4, MOV
- **Language**: Single language support (Polish initially configured)
- **Network**: Requires active internet connection (no offline mode)
- **Custom Fields**: Limited number of custom fields per organization

## User Roles

| Role | Portal | Capabilities |
|------|--------|--------------|
| **Service Administrator** | Service Portal | Full system access, user management, organization setup, all ticket operations |
| **Service Technician** | Service Portal | View and manage assigned tickets, add notes, change status, communicate with clients |
| **Organization Administrator** | Client Portal | Manage team members, view all organization tickets, reopen tickets, create tickets |
| **Machine Operator** | Client Portal | Create tickets, view own tickets, add comments and attachments |

## Project Status

🚧 **In Development** - MVP Phase

The project is currently in active development with the following milestones:

- [x] Project setup and architecture design
- [x] Technology stack selection and analysis
- [ ] Backend API development
  - [ ] Authentication & authorization
  - [ ] Ticket management endpoints
  - [ ] Organization management
  - [ ] Machine log integration
- [ ] Frontend development
  - [ ] Service Portal UI
  - [ ] Client Portal UI
  - [ ] Responsive design implementation
- [ ] Database schema and migrations
- [ ] Testing and quality assurance
- [ ] Deployment pipeline setup
- [ ] MVP release

**Target MVP Release**: Q1 2026

## Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please ensure your code follows our coding standards:
- Run `npm run lint:fix` for frontend code
- Follow .NET naming conventions for backend code
- Write unit tests for new features
- Update documentation as needed

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Developed with ❤️ by Baumalog Sp. z o.o.**

For questions or support, please contact: [support@baumalog.pl](mailto:support@baumalog.pl)
