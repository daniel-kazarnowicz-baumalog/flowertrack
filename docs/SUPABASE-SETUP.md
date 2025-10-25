# Supabase Setup Guide

This document explains how to configure Supabase integration for the FLOWerTRACK application.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Creating a Supabase Project](#creating-a-supabase-project)
- [Getting API Keys](#getting-api-keys)
- [Configuring the Application](#configuring-the-application)
- [Setting up Storage](#setting-up-storage)
- [Configuring RLS Policies](#configuring-rls-policies)
- [Testing the Connection](#testing-the-connection)
- [Troubleshooting](#troubleshooting)

## Prerequisites

- A Supabase account (sign up at [supabase.com](https://supabase.com))
- .NET 9.0 SDK installed
- PostgreSQL client (optional, for direct database access)

## Creating a Supabase Project

1. **Sign in to Supabase**
   - Navigate to [app.supabase.com](https://app.supabase.com)
   - Sign in with your account or create a new one

2. **Create a New Project**
   - Click "New Project"
   - Fill in the required information:
     - Project name: `flowertrack` (or your preferred name)
     - Database password: Choose a strong password (save this securely)
     - Region: Select the closest region to your users
   - Click "Create new project"
   - Wait for the project to be provisioned (usually 2-3 minutes)

## Getting API Keys

Once your project is created, you need to obtain the following keys:

### 1. Project URL and API Keys

Navigate to **Settings → API** in your Supabase dashboard:

- **Project URL**: `https://your-project.supabase.co`
- **Anon Key** (Public): Used for client-side operations with RLS enabled
- **Service Role Key** (Secret): Used for server-side operations, bypasses RLS

### 2. JWT Secret

Navigate to **Settings → API → JWT Settings**:

- **JWT Secret**: Used for token validation

⚠️ **Security Warning**: Never commit the Service Role Key or JWT Secret to your repository!

## Configuring the Application

### Option 1: Using User Secrets (Development - Recommended)

This is the **recommended approach** for local development:

```bash
# Navigate to the API project directory
cd src/backend/Flowertrack.Api

# Initialize user secrets (if not already done)
dotnet user-secrets init

# Set Supabase configuration
dotnet user-secrets set "Supabase:Url" "https://your-project.supabase.co"
dotnet user-secrets set "Supabase:AnonKey" "your-anon-key"
dotnet user-secrets set "Supabase:ServiceKey" "your-service-role-key"
dotnet user-secrets set "Supabase:JwtSecret" "your-jwt-secret"

# Set database connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.your-project.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-db-password"

# List all secrets to verify
dotnet user-secrets list
```

### Option 2: Using Environment Variables (Production)

For production deployment, use environment variables:

```bash
# Set environment variables (Linux/macOS)
export Supabase__Url="https://your-project.supabase.co"
export Supabase__AnonKey="your-anon-key"
export Supabase__ServiceKey="your-service-role-key"
export Supabase__JwtSecret="your-jwt-secret"
export ConnectionStrings__DefaultConnection="Host=db.your-project.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-db-password"

# For Windows PowerShell
$env:Supabase__Url = "https://your-project.supabase.co"
$env:Supabase__AnonKey = "your-anon-key"
$env:Supabase__ServiceKey = "your-service-role-key"
$env:Supabase__JwtSecret = "your-jwt-secret"
$env:ConnectionStrings__DefaultConnection = "Host=db.your-project.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-db-password"
```

### Option 3: Azure Key Vault (Production)

For cloud deployments, Azure Key Vault is recommended for storing secrets securely.

### Database Connection String Format

```
Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your-password
```

Replace:
- `xxx` with your Supabase project reference
- `your-password` with your database password

## Setting up Storage

### 1. Create Storage Bucket

1. Navigate to **Storage** in your Supabase dashboard
2. Click "New bucket"
3. Configure the bucket:
   - **Name**: `attachments`
   - **Public**: No (we'll use RLS for access control)
   - **File size limit**: 10 MB (as per MVP requirements)
   - **Allowed MIME types**: 
     - `image/jpeg`, `image/png`
     - `application/pdf`
     - `text/plain`
     - `application/msword`, `application/vnd.openxmlformats-officedocument.wordprocessingml.document`
     - `application/vnd.ms-excel`, `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
     - `application/zip`
     - `video/mp4`, `video/quicktime`
4. Click "Create bucket"

### 2. Folder Structure

Files will be organized using the following path strategy:
```
{organizationId}/{ticketId}/{filename}
```

Example:
```
123/456/screenshot-2025-10-25.png
123/456/error-log.txt
```

## Configuring RLS Policies

Row Level Security (RLS) policies control access to storage and database resources.

### Storage RLS Policies

Create the following policies in **Storage → Policies** for the `attachments` bucket:

#### 1. Allow Users to Upload to Their Organization Folder

```sql
CREATE POLICY "Users can upload to their organization folder"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
);
```

#### 2. Allow Users to Read from Their Organization Folder

```sql
CREATE POLICY "Users can read from their organization folder"
ON storage.objects FOR SELECT
TO authenticated
USING (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
);
```

#### 3. Allow Service Role Full Access

```sql
CREATE POLICY "Service role has full access"
ON storage.objects
TO service_role
USING (bucket_id = 'attachments')
WITH CHECK (bucket_id = 'attachments');
```

#### 4. Allow Users to Delete Their Organization's Files

```sql
CREATE POLICY "Users can delete their organization files"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
);
```

### Alternative: SQL Script

You can also run the policies using the SQL script located at:
`src/backend/Flowertrack.Infrastructure/Persistence/Scripts/storage-policies.sql`

## Testing the Connection

### 1. Health Check Endpoint

Once the application is running, test the Supabase connection:

```bash
# Test general health
curl http://localhost:5102/health

# Test Supabase-specific health
curl http://localhost:5102/health/supabase
```

Expected response:
```json
{
  "status": "Healthy",
  "results": {
    "supabase": {
      "status": "Healthy",
      "description": "Supabase is operational",
      "data": {
        "database": "connected",
        "supabase_api": "available"
      }
    }
  }
}
```

### 2. Test Database Connection

```bash
# From the backend directory
cd src/backend
dotnet ef database update --startup-project Flowertrack.Api --project Flowertrack.Infrastructure
```

If successful, you should see:
```
Done.
```

### 3. Test Authentication (Optional)

If you have JWT tokens set up, you can test authentication:

```bash
curl -H "Authorization: Bearer your-jwt-token" http://localhost:5102/api/protected-endpoint
```

## Troubleshooting

### Issue: "Cannot connect to PostgreSQL database"

**Possible causes:**
- Incorrect connection string
- Database password is wrong
- IP address not whitelisted in Supabase

**Solution:**
1. Verify your connection string in user secrets
2. Check if your IP is allowed in Supabase → Settings → Database → Connection Pooling
3. Verify database password

### Issue: "Supabase client not properly initialized"

**Possible causes:**
- Missing or invalid API keys
- Project URL is incorrect

**Solution:**
1. Verify all Supabase configuration values in user secrets
2. Ensure the URL format is correct: `https://your-project.supabase.co`
3. Check that API keys are from the correct project

### Issue: "JWT Secret not configured"

**Possible causes:**
- JWT Secret is missing from configuration
- Configuration not loaded properly

**Solution:**
1. Set the JWT Secret in user secrets:
   ```bash
   dotnet user-secrets set "Supabase:JwtSecret" "your-jwt-secret"
   ```
2. Restart the application

### Issue: "Storage upload fails"

**Possible causes:**
- RLS policies not configured
- Bucket doesn't exist
- File size exceeds limit

**Solution:**
1. Verify the `attachments` bucket exists
2. Check RLS policies are applied
3. Ensure file size is under 10MB

### Issue: "Authentication failed"

**Possible causes:**
- Invalid JWT token
- Token expired
- Issuer/Audience mismatch

**Solution:**
1. Verify JWT Secret is correct
2. Check token expiration
3. Ensure Supabase URL matches the issuer in JWT configuration

## Best Practices

1. **Never commit secrets**: Always use user secrets, environment variables, or Azure Key Vault
2. **Use Service Role Key carefully**: Only use it for server-side operations that need to bypass RLS
3. **Rotate keys regularly**: Change API keys and passwords periodically
4. **Monitor usage**: Check Supabase dashboard for API usage and database performance
5. **Enable RLS**: Always use Row Level Security for data protection
6. **Test in staging**: Test all configuration changes in a staging environment first

## Additional Resources

- [Supabase Documentation](https://supabase.com/docs)
- [Supabase Auth Guide](https://supabase.com/docs/guides/auth)
- [Supabase Storage Guide](https://supabase.com/docs/guides/storage)
- [Row Level Security](https://supabase.com/docs/guides/auth/row-level-security)
- [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

## Support

If you encounter issues not covered in this guide:
1. Check Supabase status at [status.supabase.com](https://status.supabase.com)
2. Review application logs in `src/backend/Flowertrack.Api/logs/`
3. Contact the development team

---

**Last Updated**: October 25, 2025  
**Version**: 1.0.0  
**Maintainer**: FLOWerTRACK Development Team
