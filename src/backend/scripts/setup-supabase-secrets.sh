#!/bin/bash

# ====================================================================
# Supabase Configuration Setup Script for FLOWerTRACK
# ====================================================================
# 
# This script helps you set up Supabase configuration using .NET User Secrets.
# This is the recommended approach for local development.
#
# Usage: 
#   ./setup-supabase-secrets.sh
#
# Prerequisites:
#   - .NET 9.0 SDK installed
#   - Supabase project created
#   - Access to Supabase project API keys
# 
# ====================================================================

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Project directory
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../Flowertrack.Api" && pwd)"

echo -e "${BLUE}=====================================================================${NC}"
echo -e "${BLUE}   FLOWerTRACK - Supabase Configuration Setup${NC}"
echo -e "${BLUE}=====================================================================${NC}"
echo ""

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ Error: .NET SDK is not installed${NC}"
    echo "Please install .NET 9.0 SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi

echo -e "${GREEN}✓ .NET SDK found${NC}"
echo ""

# Navigate to project directory
cd "$PROJECT_DIR"
echo -e "${BLUE}Working directory: ${PROJECT_DIR}${NC}"
echo ""

# Initialize user secrets if not already done
echo -e "${YELLOW}Initializing user secrets...${NC}"
dotnet user-secrets init --project "$PROJECT_DIR" || true
echo -e "${GREEN}✓ User secrets initialized${NC}"
echo ""

# Prompt for Supabase configuration
echo -e "${BLUE}=====================================================================${NC}"
echo -e "${BLUE}   Enter your Supabase configuration${NC}"
echo -e "${BLUE}=====================================================================${NC}"
echo ""
echo "You can find these values in your Supabase dashboard:"
echo "  → Settings → API"
echo ""

read -p "Supabase Project URL (e.g., https://xxx.supabase.co): " supabase_url
read -p "Supabase Anon Key (Public): " supabase_anon_key
read -s -p "Supabase Service Role Key (Secret): " supabase_service_key
echo ""
read -s -p "Supabase JWT Secret: " supabase_jwt_secret
echo ""
echo ""

echo -e "${BLUE}=====================================================================${NC}"
echo -e "${BLUE}   Enter your Database connection details${NC}"
echo -e "${BLUE}=====================================================================${NC}"
echo ""
echo "Format: Host=db.xxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=xxx"
echo ""

read -p "Database Host (e.g., db.xxx.supabase.co): " db_host
read -p "Database Port [5432]: " db_port
db_port=${db_port:-5432}

read -p "Database Name [postgres]: " db_name
db_name=${db_name:-postgres}

read -p "Database Username [postgres]: " db_user
db_user=${db_user:-postgres}

read -s -p "Database Password: " db_password
echo ""
echo ""

# Build connection string
connection_string="Host=${db_host};Port=${db_port};Database=${db_name};Username=${db_user};Password=${db_password}"

# Set secrets
echo -e "${YELLOW}Setting user secrets...${NC}"
echo ""

dotnet user-secrets set "Supabase:Url" "$supabase_url" --project "$PROJECT_DIR"
dotnet user-secrets set "Supabase:AnonKey" "$supabase_anon_key" --project "$PROJECT_DIR"
dotnet user-secrets set "Supabase:ServiceKey" "$supabase_service_key" --project "$PROJECT_DIR"
dotnet user-secrets set "Supabase:JwtSecret" "$supabase_jwt_secret" --project "$PROJECT_DIR"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "$connection_string" --project "$PROJECT_DIR"

echo ""
echo -e "${GREEN}✓ All secrets configured successfully!${NC}"
echo ""

# Display configured secrets (without values)
echo -e "${BLUE}=====================================================================${NC}"
echo -e "${BLUE}   Configured Secrets${NC}"
echo -e "${BLUE}=====================================================================${NC}"
echo ""
dotnet user-secrets list --project "$PROJECT_DIR" || true
echo ""

# Test connection (optional)
echo -e "${YELLOW}Would you like to test the database connection? (y/n)${NC}"
read -p "> " test_connection

if [[ "$test_connection" == "y" || "$test_connection" == "Y" ]]; then
    echo ""
    echo -e "${YELLOW}Testing database connection...${NC}"
    cd "$PROJECT_DIR"
    
    # Try to build the project
    if dotnet build --no-restore > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Project builds successfully${NC}"
        
        # Try to run migrations
        echo -e "${YELLOW}Applying database migrations...${NC}"
        if dotnet ef database update 2>&1 | grep -q "Done"; then
            echo -e "${GREEN}✓ Database connection successful!${NC}"
        else
            echo -e "${RED}❌ Database connection failed. Please check your connection string.${NC}"
        fi
    else
        echo -e "${YELLOW}⚠ Project build failed. Please ensure all dependencies are installed.${NC}"
    fi
fi

echo ""
echo -e "${BLUE}=====================================================================${NC}"
echo -e "${GREEN}   Setup Complete!${NC}"
echo -e "${BLUE}=====================================================================${NC}"
echo ""
echo "Next steps:"
echo "  1. Review docs/SUPABASE-SETUP.md for detailed configuration"
echo "  2. Set up Storage bucket 'attachments' in Supabase dashboard"
echo "  3. Configure RLS policies (see storage-policies.sql)"
echo "  4. Run: dotnet run --project Flowertrack.Api"
echo "  5. Test health endpoint: curl http://localhost:5102/health/supabase"
echo ""
echo -e "${GREEN}Happy coding! 🚀${NC}"
echo ""
