# 🧪 Test Users for Development

This directory contains scripts to create test users for development and testing purposes.

## 📋 Quick Start

### Option 1: Automated Setup (Recommended)

```powershell
cd src/backend/scripts
.\setup-test-users.ps1
```

### Option 2: Manual SQL Execution

```powershell
# Using psql
psql $env:SUPABASE_CONNECTION_STRING -f create-test-users.sql

# Or copy the SQL and execute in Supabase SQL Editor
```

## 🔐 Test Credentials

### Service Portal (http://localhost:5173/service/login)

| Role | Email | Password |
|------|-------|----------|
| **Administrator** | `admin@flowertrack.dev` | `Admin123!` |
| **Technician** | `tech@flowertrack.dev` | `Tech123!` |

### Client Portal (http://localhost:5173/client/login)

| Role | Email | Password |
|------|-------|----------|
| **Organization Admin** | `client@test.com` | `Client123!` |
| **Operator** | `operator@test.com` | `Operator123!` |

## 🏢 Test Organization

- **Name:** Test Organization
- **Contact Email:** contact@test.com
- **Contact Phone:** +48 123 456 789
- **API Key:** test-api-key-12345

## 🔧 Test Machines

Two sample machines are created for the test organization:

1. **FlowMaster 3000** (Serial: FM3000-001) - Active
2. **FlowMaster 3000** (Serial: FM3000-002) - Active

## 🚀 Running the Application

### Start Backend

```powershell
cd src/backend/Presentation/Flowertrack.Api
dotnet run
```

Backend will be available at: **http://localhost:5102**

### Start Frontend

```powershell
cd src/frontend/flowertrack-client
npm run dev
```

Frontend will be available at: **http://localhost:5173**

## 🧪 Testing Workflows

### Service Portal Testing

1. Login as **admin@flowertrack.dev**
2. Navigate to Dashboard
3. Access all menu items (Dashboard, Tickets, Organizations, Admin)
4. Test logout

### Client Portal Testing

1. Login as **client@test.com** (Admin)
2. Navigate to Dashboard
3. Access menu items (Dashboard, Tickets, Team)
4. Test logout

### Role-Based Access Testing

1. Login as **tech@flowertrack.dev** (Technician)
2. Try accessing `/service/admin` - should redirect to dashboard
3. Verify only Dashboard, Tickets, Organizations are visible

4. Login as **operator@test.com** (Operator)
5. Check that Team menu is NOT visible (only for org admins)

## 🔒 Password Hashing

All passwords are hashed using BCrypt with work factor 11:

```csharp
BCrypt.Net.BCrypt.HashPassword("Admin123!", 11)
```

## 📝 Notes

- These are **development/testing credentials only**
- **DO NOT use in production**
- The SQL script uses `ON CONFLICT` clauses, so it's safe to run multiple times
- All test data uses predictable UUIDs for easy reference

## 🗑️ Cleaning Up Test Data

```sql
-- Remove test users
DELETE FROM "ServiceUsers" WHERE "Email" LIKE '%flowertrack.dev';
DELETE FROM "OrganizationUsers" WHERE "Email" LIKE '%test.com';
DELETE FROM "Organizations" WHERE "Name" = 'Test Organization';
DELETE FROM "Machines" WHERE "SerialNumber" LIKE 'FM3000-%';
```

## 🐛 Troubleshooting

### "Connection string not found"

Make sure `SUPABASE_CONNECTION_STRING` is set in your environment or in `appsettings.Development.json`.

### "psql command not found"

Install PostgreSQL client tools or execute the SQL manually in Supabase SQL Editor.

### "Cannot login with test credentials"

1. Verify the SQL script executed successfully
2. Check database for users: `SELECT * FROM "ServiceUsers";`
3. Verify backend is running and accessible
4. Check browser console for errors
