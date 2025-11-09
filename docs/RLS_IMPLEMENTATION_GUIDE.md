# Row Level Security (RLS) Implementation Guide

## Overview

Row Level Security (RLS) ensures data isolation between organizations and proper access control for service team members. This document describes the complete RLS implementation for FLOWerTRACK.

**Implementation Date:** November 9, 2025  
**Phase:** Backend MVP - Phase 2  
**Status:** Complete for existing tables, prepared for future tables

---

## Migration Files (Apply in Order)

All SQL migrations are located in `src/backend/scripts/`:

1. **20251109_enable_rls.sql** - Enable RLS and create helper functions
2. **20251109_rls_organizations.sql** - Organizations table policies
3. **20251109_rls_organization_users.sql** - OrganizationUsers table policies
4. **20251109_rls_service_users_machines.sql** - ServiceUsers and Machines table policies
5. **20251109_rls_tickets.sql** - Tickets table policies
6. **20251109_rls_comments_attachments.sql** - Future: TicketComments and TicketAttachments policies

---

## How to Apply Migrations

### Option 1: Supabase Dashboard (Recommended for Development)

1. Open Supabase Dashboard → SQL Editor
2. Copy content of each migration file in order
3. Execute each migration
4. Verify success by checking for errors

### Option 2: psql Command Line

```bash
# Connect to your Supabase database
psql "postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres"

# Apply migrations in order
\i src/backend/scripts/20251109_enable_rls.sql
\i src/backend/scripts/20251109_rls_organizations.sql
\i src/backend/scripts/20251109_rls_organization_users.sql
\i src/backend/scripts/20251109_rls_service_users_machines.sql
\i src/backend/scripts/20251109_rls_tickets.sql
\i src/backend/scripts/20251109_rls_comments_attachments.sql
```

### Option 3: Supabase CLI

```bash
# Ensure you're logged in
supabase login

# Link to your project
supabase link --project-ref [YOUR-PROJECT-REF]

# Apply migrations (if using Supabase migrations folder)
supabase db push
```

---

## Helper Functions Created

The following SQL functions are created in **20251109_enable_rls.sql**:

| Function | Returns | Description |
|----------|---------|-------------|
| `auth.uid()` | UUID | Current user's Supabase UID from JWT |
| `auth.user_role()` | TEXT | Current user's role from JWT claims |
| `public.is_service_user()` | BOOLEAN | True if user is an active service user |
| `public.is_service_admin()` | BOOLEAN | True if user has `service_admin` role |
| `public.is_organization_admin()` | BOOLEAN | True if user is org admin/owner |
| `public.is_organization_owner()` | BOOLEAN | True if user is org owner |
| `public.current_user_organization_id()` | UUID | Current user's organization ID |
| `public.user_belongs_to_organization(UUID)` | BOOLEAN | True if user belongs to specified org |
| `public.user_has_ticket_access(UUID)` | BOOLEAN | True if user can access the ticket |

---

## Access Control Summary

### Organizations Table

| Operation | Service Admin | Service User | Org Owner | Org Admin | Org User |
|-----------|--------------|--------------|-----------|-----------|----------|
| View All Organizations | ✅ | ✅ | ❌ | ❌ | ❌ |
| View Own Organization | ✅ | ✅ | ✅ | ✅ | ✅ |
| Create Organization | ✅ | ❌ | ❌ | ❌ | ❌ |
| Update Any Organization | ✅ | ❌ | ❌ | ❌ | ❌ |
| Update Own Organization | ✅ | ❌ | ✅ | ❌ | ❌ |
| Delete Organization | ✅ | ❌ | ❌ | ❌ | ❌ |

### OrganizationUsers Table

| Operation | Service Admin | Service User | Org Owner | Org Admin | Org User |
|-----------|--------------|--------------|-----------|-----------|----------|
| View All Users | ✅ | ✅ | ❌ | ❌ | ❌ |
| View Org Users | ✅ | ✅ | ✅ | ✅ | ✅ |
| View Own Profile | ✅ | ✅ | ✅ | ✅ | ✅ |
| Invite User to Org | ✅ | ❌ | ✅ | ✅ | ❌ |
| Update Any User | ✅ | ❌ | ❌ | ❌ | ❌ |
| Update Org Users | ✅ | ❌ | ✅ | ✅ | ❌ |
| Update Own Profile | ✅ | ✅ | ✅ | ✅ | ✅ |
| Delete Org Users | ✅ | ❌ | ✅* | ✅* | ❌ |

*Cannot delete org owners

### ServiceUsers Table

| Operation | Service Admin | Service User | Org User |
|-----------|--------------|--------------|----------|
| View All Service Users | ✅ | ✅ | ❌ |
| View Own Profile | ✅ | ✅ | ❌ |
| Create Service User | ✅ | ❌ | ❌ |
| Update Any Service User | ✅ | ❌ | ❌ |
| Update Own Profile | ✅ | ✅ | ❌ |
| Delete Service User | ✅ | ❌ | ❌ |

### Machines Table

| Operation | Service Admin | Service User | Org Admin | Org User |
|-----------|--------------|--------------|-----------|----------|
| View All Machines | ✅ | ✅ | ❌ | ❌ |
| View Org Machines | ✅ | ✅ | ✅ | ✅ |
| Create Machine (Any Org) | ✅ | ❌ | ❌ | ❌ |
| Create Machine (Own Org) | ✅ | ❌ | ✅ | ❌ |
| Update Any Machine | ✅ | ✅ | ❌ | ❌ |
| Update Org Machines | ✅ | ✅ | ✅ | ❌ |
| Delete Any Machine | ✅ | ❌ | ❌ | ❌ |
| Delete Org Machines | ✅ | ❌ | ✅ | ❌ |

### Tickets Table

| Operation | Service Admin | Service User | Org Admin | Org User |
|-----------|--------------|--------------|-----------|----------|
| View All Tickets | ✅ | ✅ | ❌ | ❌ |
| View Org Tickets | ✅ | ✅ | ✅ | ✅ |
| Create Ticket (Any Org) | ✅ | ✅ | ❌ | ❌ |
| Create Ticket (Own Org) | ✅ | ✅ | ✅ | ✅ |
| Update Any Ticket | ✅ | ✅ | ❌ | ❌ |
| Update Org Tickets* | ✅ | ✅ | ✅* | ✅* |
| Delete Any Ticket | ✅ | ❌ | ❌ | ❌ |
| Delete New Tickets | ✅ | ❌ | ✅ | ❌ |

*Organization users can update tickets but application layer restricts which fields (cannot change Status, AssignedToUserId, Priority)

### TicketComments & TicketAttachments (Future)

| Operation | Service Admin | Service User | Org User (on accessible ticket) |
|-----------|--------------|--------------|----------------------------------|
| View Comments/Attachments | ✅ | ✅ | ✅ |
| Create Comment/Attachment | ✅ | ✅ | ✅ |
| Update Own Comment | ✅ | ✅ | ✅ (time-limited) |
| Update Any Comment | ✅ | ❌ | ❌ |
| Delete Own Comment/Attachment | ✅ | ✅ | ✅ |
| Delete Any Comment/Attachment | ✅ | ❌ | ❌ |

---

## Verification Queries

After applying migrations, verify RLS is enabled:

```sql
-- Check RLS status on all tables
SELECT tablename, rowsecurity 
FROM pg_tables 
WHERE schemaname = 'public' 
AND tablename IN (
    'Organizations', 
    'OrganizationUsers', 
    'ServiceUsers', 
    'Machines', 
    'Tickets'
)
ORDER BY tablename;

-- Expected: All should have rowsecurity = true
```

View all policies:

```sql
-- List all policies
SELECT schemaname, tablename, policyname, permissive, roles, cmd
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;
```

Test isolation (requires actual data):

```sql
-- Test as organization user (should see only their org)
SET request.jwt.claims = '{"sub": "org-user-uuid", "role": "authenticated"}';
SELECT COUNT(*) FROM "Organizations"; -- Should return 1
SELECT COUNT(*) FROM "Machines"; -- Should return machines from their org only

-- Test as service user (should see all)
SET request.jwt.claims = '{"sub": "service-user-uuid", "role": "service_user"}';
SELECT COUNT(*) FROM "Organizations"; -- Should return all organizations
SELECT COUNT(*) FROM "Machines"; -- Should return all machines
```

---

## Security Guarantees

1. **Data Isolation**: Organization users can only see data from their organization
2. **Service Visibility**: Service team has full visibility for support
3. **Hierarchical Access**: Admins have more permissions than regular users
4. **Owner Protection**: Organization owners cannot be deleted by org admins
5. **Soft Deletes**: All deletes are soft deletes (`IsDeleted = true`)
6. **Activation Flow**: Inactive users can still see their profile for activation
7. **Ticket Access**: Comments/attachments inherit ticket access permissions

---

## Application Layer Responsibilities

RLS handles **row-level** access, but application must enforce **column-level** restrictions:

### Tickets
- ❌ Org users cannot change: `Status`, `AssignedToUserId`, `Priority`
- ✅ Org users can change: `Description`, add comments

### OrganizationUsers
- ❌ Users cannot change: `Role`, `OrganizationId`, `SupabaseUserId`
- ✅ Users can change: `FirstName`, `LastName`, `PhoneNumber`

### Comments
- ✅ Users can edit own comments within 15 minutes of creation
- ❌ Cannot edit after time limit expires

---

## Troubleshooting

### Issue: No rows returned after applying RLS

**Cause**: Policies are missing or JWT claims are incorrect

**Solution**: 
1. Check JWT token contains correct claims (`sub`, `role`)
2. Verify user exists in `ServiceUsers` or `OrganizationUsers` table
3. Verify `SupabaseUserId` matches JWT `sub` claim
4. Check `IsDeleted = false` and `IsActivated = true` (for org users)

### Issue: "permission denied for table"

**Cause**: GRANT statements not executed

**Solution**: Re-run the GRANT statements from each migration file

### Issue: Service users see no data

**Cause**: `is_service_user()` function not working

**Solution**:
1. Check `ServiceUsers` table has user with matching `SupabaseUserId`
2. Verify `Status = 'Active'` and `IsDeleted = false`
3. Test function directly: `SELECT public.is_service_user();`

---

## Future Enhancements

When adding new tables:

1. Enable RLS immediately: `ALTER TABLE "NewTable" ENABLE ROW LEVEL SECURITY;`
2. Create SELECT policies for service users and org users
3. Create INSERT/UPDATE/DELETE policies with proper checks
4. Add GRANT statements: `GRANT SELECT, INSERT, UPDATE ON "NewTable" TO authenticated;`
5. Add verification queries to migration file
6. Update this documentation

---

## References

- Supabase RLS Documentation: https://supabase.com/docs/guides/auth/row-level-security
- PostgreSQL RLS Documentation: https://www.postgresql.org/docs/current/ddl-rowsecurity.html
- Project Architecture: `README.md`
- Supabase Setup: `docs/SUPABASE-SETUP.md`

---

**Last Updated:** November 9, 2025  
**Maintained By:** Backend Team
