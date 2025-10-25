# Backend Scripts

This directory contains utility scripts for setting up and managing the FLOWerTRACK backend.

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
