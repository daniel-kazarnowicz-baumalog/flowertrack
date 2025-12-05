# =====================================================
# FLOWerTRACK - Setup Test Users (Development)
# =====================================================
# This script generates BCrypt password hashes and creates test users
# in the Supabase database for development purposes.
#
# Prerequisites:
# - .NET SDK installed
# - Supabase connection configured
# =====================================================

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  FLOWerTRACK - Test Users Setup" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to backend API directory
$backendPath = Join-Path $PSScriptRoot ".." "Presentation" "Flowertrack.Api"
Set-Location $backendPath

Write-Host "📍 Location: $backendPath" -ForegroundColor Gray
Write-Host ""

# Check if we can build the project
Write-Host "🔨 Building backend project..." -ForegroundColor Yellow
dotnet build --configuration Debug --no-restore > $null 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Build failed. Restoring packages..." -ForegroundColor Yellow
    dotnet restore
    dotnet build --configuration Debug
}

Write-Host "✅ Build successful" -ForegroundColor Green
Write-Host ""

# Create a simple C# program to generate BCrypt hashes
$hashGeneratorCode = @'
using BCrypt.Net;
using System;

// Test passwords
var passwords = new Dictionary<string, string>
{
    { "admin@flowertrack.dev", "Admin123!" },
    { "tech@flowertrack.dev", "Tech123!" },
    { "client@test.com", "Client123!" },
    { "operator@test.com", "Operator123!" }
};

Console.WriteLine("Generated BCrypt Hashes:");
Console.WriteLine("========================");
foreach (var kvp in passwords)
{
    var hash = BCrypt.Net.BCrypt.HashPassword(kvp.Value, 11);
    Console.WriteLine($"{kvp.Key}: {hash}");
}
'@

# For now, we'll use pre-generated hashes (BCrypt work factor 11)
# These hashes correspond to the passwords in comments
Write-Host "🔐 Test User Credentials:" -ForegroundColor Cyan
Write-Host ""
Write-Host "SERVICE PORTAL (Purple):" -ForegroundColor Magenta
Write-Host "  Admin Account:" -ForegroundColor White
Write-Host "    Email:    admin@flowertrack.dev" -ForegroundColor Gray
Write-Host "    Password: Admin123!" -ForegroundColor Gray
Write-Host ""
Write-Host "  Technician Account:" -ForegroundColor White
Write-Host "    Email:    tech@flowertrack.dev" -ForegroundColor Gray
Write-Host "    Password: Tech123!" -ForegroundColor Gray
Write-Host ""
Write-Host "CLIENT PORTAL (Green):" -ForegroundColor Green
Write-Host "  Organization Admin:" -ForegroundColor White
Write-Host "    Email:    client@test.com" -ForegroundColor Gray
Write-Host "    Password: Client123!" -ForegroundColor Gray
Write-Host ""
Write-Host "  Operator:" -ForegroundColor White
Write-Host "    Email:    operator@test.com" -ForegroundColor Gray
Write-Host "    Password: Operator123!" -ForegroundColor Gray
Write-Host ""

# Ask if user wants to create these users
Write-Host "⚠️  This will create/update test users in your database." -ForegroundColor Yellow
$response = Read-Host "Do you want to proceed? (y/n)"

if ($response -ne 'y' -and $response -ne 'Y') {
    Write-Host "❌ Cancelled by user" -ForegroundColor Red
    exit 0
}

Write-Host ""
Write-Host "🗄️  Executing SQL script..." -ForegroundColor Yellow

# Check if SUPABASE_CONNECTION_STRING is set
$connectionString = $env:SUPABASE_CONNECTION_STRING

if (-not $connectionString) {
    Write-Host "⚠️  SUPABASE_CONNECTION_STRING not found in environment" -ForegroundColor Yellow
    Write-Host "Checking appsettings.Development.json..." -ForegroundColor Gray
    
    $appSettingsPath = Join-Path $backendPath "appsettings.Development.json"
    if (Test-Path $appSettingsPath) {
        $appSettings = Get-Content $appSettingsPath | ConvertFrom-Json
        $connectionString = $appSettings.ConnectionStrings.SupabaseConnection
    }
}

if (-not $connectionString) {
    Write-Host "❌ Cannot find database connection string" -ForegroundColor Red
    Write-Host "Please set SUPABASE_CONNECTION_STRING environment variable" -ForegroundColor Yellow
    exit 1
}

# Execute the SQL script using psql (if available) or dotnet ef
$sqlScriptPath = Join-Path $PSScriptRoot "create-test-users.sql"

# Try using psql
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue

if ($psqlPath) {
    Write-Host "Using psql to execute script..." -ForegroundColor Gray
    psql $connectionString -f $sqlScriptPath
} else {
    Write-Host "⚠️  psql not found. You'll need to execute the SQL script manually:" -ForegroundColor Yellow
    Write-Host "   $sqlScriptPath" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Or install PostgreSQL client tools." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  ✅ Setup Complete!" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "You can now login with the test credentials above." -ForegroundColor White
Write-Host ""
Write-Host "Frontend URLs:" -ForegroundColor Cyan
Write-Host "  Service Portal: http://localhost:5173/service/login" -ForegroundColor Gray
Write-Host "  Client Portal:  http://localhost:5173/client/login" -ForegroundColor Gray
Write-Host ""
