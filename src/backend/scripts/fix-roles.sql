-- =====================================================
-- FLOWerTRACK - Fix Roles and UserRoles
-- =====================================================

BEGIN;

-- 1. Ensure Roles exist
INSERT INTO "Roles" ("Id", "Name", "Description")
VALUES 
(1, 'ServiceAdministrator', 'Service team administrator with full access to service management'),
(2, 'ServiceTechnician', 'Service technician with ticket management and resolution capabilities'),
(3, 'OrganizationAdministrator', 'Organization administrator with team and machine management access'),
(4, 'Operator', 'Machine operator with basic ticket creation and viewing capabilities')
ON CONFLICT ("Id") DO UPDATE SET
    "Name" = EXCLUDED."Name",
    "Description" = EXCLUDED."Description";

-- 2. Assign ServiceAdministrator role to Admin User
-- Admin User ID: 660e8400-e29b-41d4-a716-446655440001
INSERT INTO "UserRoles" ("Id", "UserId", "RoleId")
VALUES (
    '990e8400-e29b-41d4-a716-446655440001', -- Fixed UUID for idempotency
    '660e8400-e29b-41d4-a716-446655440001', -- Admin User ID
    1 -- ServiceAdministrator Role ID
)
ON CONFLICT DO NOTHING;

-- 3. Assign ServiceTechnician role to Tech User
-- Tech User ID: 660e8400-e29b-41d4-a716-446655440002
INSERT INTO "UserRoles" ("Id", "UserId", "RoleId")
VALUES (
    '990e8400-e29b-41d4-a716-446655440002', -- Fixed UUID for idempotency
    '660e8400-e29b-41d4-a716-446655440002', -- Tech User ID
    2 -- ServiceTechnician Role ID
)
ON CONFLICT DO NOTHING;

COMMIT;
