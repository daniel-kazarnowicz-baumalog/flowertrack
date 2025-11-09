# Backend Scripts

This directory contains utility scripts and SQL migrations for setting up and managing the FLOWerTRACK backend.

## Directory Contents

### SQL Migrations
- `init-database.sql` - Initial database setup
- `20251109_enable_rls.sql` - Phase 2.1: Enable Row Level Security on all tables

### Setup Scripts
- `setup-supabase-secrets.ps1` / `setup-supabase-secrets.sh` - Configure Supabase integration

## SQL Migration Order

Execute SQL migrations in this order:

1. **init-database.sql** - Initial database schema (run first)
2. **20251109_enable_rls.sql** - Phase 2.1: Enable RLS and create helper functions
3. **20251109_rls_organizations.sql** - Phase 2.2: Organizations table policies
4. **20251109_rls_organization_users.sql** - Phase 2.3: OrganizationUsers table policies
5. **20251109_rls_service_users_machines.sql** - Phase 2.4: ServiceUsers and Machines policies
6. **20251109_rls_tickets.sql** - Phase 2.5: Tickets table policies
7. **20251109_rls_comments_attachments.sql** - Phase 2.6: Future comments/attachments policies

### Applying Migrations

**Option 1: Supabase Dashboard**
1. Open SQL Editor in Supabase Dashboard
2. Copy and execute each file in order
3. Verify no errors in the output

**Option 2: psql**
```bash
psql "postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres"
\i src/backend/scripts/20251109_enable_rls.sql
# ... continue with remaining files
```

**For detailed RLS documentation, see:** `docs/RLS_IMPLEMENTATION_GUIDE.md`

## Available Scripts

### 1. Supabase Setup Scripts

These scripts help you configure Supabase integration using .NET User Secrets (recommended for development).

#### Linux/macOS:
```bash
cd src/backend/scripts
./setup-supabase-secrets.sh
```

#### Windows (PowerShell):
```powershell
cd src\backend\scripts
.\setup-supabase-secrets.ps1
```

**What these scripts do:**
- Initialize .NET User Secrets for the API project
- Prompt you for Supabase configuration (URL, API keys, JWT secret)
- Prompt you for database connection details
- Securely store all secrets using .NET User Secrets
- Optionally test the database connection

**Prerequisites:**
- .NET 9.0 SDK installed
- A Supabase project created
- Access to your Supabase project's API keys (found in Settings → API)

**After running:**
1. Review the complete setup guide: `docs/SUPABASE-SETUP.md`
2. Create the `attachments` storage bucket in Supabase
3. Apply RLS policies from `src/backend/Flowertrack.Infrastructure/Persistence/Scripts/storage-policies.sql`
4. Run the application: `dotnet run --project Flowertrack.Api`
5. Test the health endpoint: `curl http://localhost:5102/health/supabase`

## Manual Configuration

If you prefer not to use the scripts, you can manually set secrets:

```bash
cd src/backend/Flowertrack.Api
dotnet user-secrets init
dotnet user-secrets set "Supabase:Url" "your-value"
dotnet user-secrets set "Supabase:AnonKey" "your-value"
dotnet user-secrets set "Supabase:ServiceKey" "your-value"
dotnet user-secrets set "Supabase:JwtSecret" "your-value"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

## Troubleshooting

### Script Execution Issues

**Linux/macOS:**
- If you get "Permission denied", run: `chmod +x setup-supabase-secrets.sh`

**Windows:**
- If you get "execution policy" error, run PowerShell as Administrator and execute:
  ```powershell
  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
  ```

### Connection Issues

If the database connection test fails:
1. Verify your Supabase project is active
2. Check that the connection string format is correct
3. Ensure your IP address is allowed in Supabase (Settings → Database → Connection Pooling)
4. Verify the database password is correct

## Security Notes

⚠️ **Important:**
- Never commit secrets to the repository
- User Secrets are stored locally in your user profile
- For production, use environment variables or Azure Key Vault
- The Service Role Key should only be used server-side

## Additional Resources

- [Complete Supabase Setup Guide](../../../docs/SUPABASE-SETUP.md)
- [.NET User Secrets Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Supabase Documentation](https://supabase.com/docs)
