# ====================================================================
# Supabase Configuration Setup Script for FLOWerTRACK (PowerShell)
# ====================================================================
# 
# This script helps you set up Supabase configuration using .NET User Secrets.
# This is the recommended approach for local development.
#
# Usage: 
#   .\setup-supabase-secrets.ps1
#
# Prerequisites:
#   - .NET 9.0 SDK installed
#   - Supabase project created
#   - Access to Supabase project API keys
# 
# ====================================================================

$ErrorActionPreference = "Stop"

# Project directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectDir = Join-Path $ScriptDir "..\Flowertrack.Api"
$ProjectDir = Resolve-Path $ProjectDir

Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host "   FLOWerTRACK - Supabase Configuration Setup" -ForegroundColor Blue
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host ""

# Check if dotnet is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK found (version $dotnetVersion)" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error: .NET SDK is not installed" -ForegroundColor Red
    Write-Host "Please install .NET 9.0 SDK from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Navigate to project directory
Set-Location $ProjectDir
Write-Host "Working directory: $ProjectDir" -ForegroundColor Blue
Write-Host ""

# Initialize user secrets if not already done
Write-Host "Initializing user secrets..." -ForegroundColor Yellow
try {
    dotnet user-secrets init --project $ProjectDir 2>&1 | Out-Null
}
catch {
    # Ignore if already initialized
}
Write-Host "✓ User secrets initialized" -ForegroundColor Green
Write-Host ""

# Prompt for Supabase configuration
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host "   Enter your Supabase configuration" -ForegroundColor Blue
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host ""
Write-Host "You can find these values in your Supabase dashboard:"
Write-Host "  → Settings → API"
Write-Host ""

$supabaseUrl = Read-Host "Supabase Project URL (e.g., https://xxx.supabase.co)"
$supabaseAnonKey = Read-Host "Supabase Anon Key (Public)"
$supabaseServiceKey = Read-Host "Supabase Service Role Key (Secret)" -AsSecureString
$supabaseServiceKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($supabaseServiceKey))

$supabaseJwtSecret = Read-Host "Supabase JWT Secret" -AsSecureString
$supabaseJwtSecretPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($supabaseJwtSecret))

Write-Host ""
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host "   Enter your Database connection details" -ForegroundColor Blue
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host ""
Write-Host "Format: Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=xxx"
Write-Host ""

$dbHost = Read-Host "Database Host (e.g., db.xxx.supabase.co)"
$dbPort = Read-Host "Database Port [5432]"
if ([string]::IsNullOrWhiteSpace($dbPort)) { $dbPort = "5432" }

$dbName = Read-Host "Database Name [postgres]"
if ([string]::IsNullOrWhiteSpace($dbName)) { $dbName = "postgres" }

$dbUser = Read-Host "Database Username [postgres]"
if ([string]::IsNullOrWhiteSpace($dbUser)) { $dbUser = "postgres" }

$dbPassword = Read-Host "Database Password" -AsSecureString
$dbPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($dbPassword))

Write-Host ""

# Build connection string
$connectionString = "Host=$dbHost;Port=$dbPort;Database=$dbName;Username=$dbUser;Password=$dbPasswordPlain"

# Set secrets
Write-Host "Setting user secrets..." -ForegroundColor Yellow
Write-Host ""

dotnet user-secrets set "Supabase:Url" $supabaseUrl --project $ProjectDir
dotnet user-secrets set "Supabase:AnonKey" $supabaseAnonKey --project $ProjectDir
dotnet user-secrets set "Supabase:ServiceKey" $supabaseServiceKeyPlain --project $ProjectDir
dotnet user-secrets set "Supabase:JwtSecret" $supabaseJwtSecretPlain --project $ProjectDir
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString --project $ProjectDir

Write-Host ""
Write-Host "✓ All secrets configured successfully!" -ForegroundColor Green
Write-Host ""

# Display configured secrets (without values)
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host "   Configured Secrets" -ForegroundColor Blue
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host ""
try {
    dotnet user-secrets list --project $ProjectDir
}
catch {
    Write-Host "Unable to list secrets" -ForegroundColor Yellow
}
Write-Host ""

# Test connection (optional)
$testConnection = Read-Host "Would you like to test the database connection? (y/n)"

if ($testConnection -eq "y" -or $testConnection -eq "Y") {
    Write-Host ""
    Write-Host "Testing database connection..." -ForegroundColor Yellow
    Set-Location $ProjectDir
    
    # Try to build the project
    try {
        dotnet build --no-restore *> $null
        Write-Host "✓ Project builds successfully" -ForegroundColor Green
        
        # Try to run migrations
        Write-Host "Applying database migrations..." -ForegroundColor Yellow
        $migrationOutput = dotnet ef database update 2>&1 | Out-String
        if ($migrationOutput -match "Done") {
            Write-Host "✓ Database connection successful!" -ForegroundColor Green
        }
        else {
            Write-Host "❌ Database connection failed. Please check your connection string." -ForegroundColor Red
        }
    }
    catch {
        Write-Host "⚠ Project build failed. Please ensure all dependencies are installed." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host "   Setup Complete!" -ForegroundColor Green
Write-Host "=====================================================================" -ForegroundColor Blue
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Review docs/SUPABASE-SETUP.md for detailed configuration"
Write-Host "  2. Set up Storage bucket 'attachments' in Supabase dashboard"
Write-Host "  3. Configure RLS policies (see storage-policies.sql)"
Write-Host "  4. Run: dotnet run --project Flowertrack.Api"
Write-Host "  5. Test health endpoint: curl http://localhost:5102/health/supabase"
Write-Host ""
Write-Host "Happy coding!" -ForegroundColor Green
Write-Host ""
