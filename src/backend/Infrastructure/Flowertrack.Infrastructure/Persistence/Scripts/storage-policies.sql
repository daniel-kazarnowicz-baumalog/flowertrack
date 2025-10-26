-- =====================================================
-- Supabase Storage RLS Policies for FLOWerTRACK
-- =====================================================
-- 
-- This script creates Row Level Security (RLS) policies
-- for the 'attachments' storage bucket.
--
-- Folder structure: {organizationId}/{ticketId}/{filename}
-- 
-- Prerequisites:
-- 1. Storage bucket 'attachments' must exist
-- 2. Users must be authenticated via Supabase Auth
-- 3. JWT token must contain 'organization_id' claim
-- 
-- =====================================================

-- Enable RLS on storage.objects if not already enabled
ALTER TABLE storage.objects ENABLE ROW LEVEL SECURITY;

-- =====================================================
-- Policy 1: Allow users to upload files to their organization folder
-- =====================================================
-- Users can INSERT (upload) files only to folders matching their organization_id
-- Path format: attachments/{organization_id}/{ticket_id}/{filename}

CREATE POLICY "Users can upload to their organization folder"
ON storage.objects 
FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
);

-- =====================================================
-- Policy 2: Allow users to read files from their organization folder
-- =====================================================
-- Users can SELECT (read/download) files only from folders matching their organization_id

CREATE POLICY "Users can read from their organization folder"
ON storage.objects 
FOR SELECT
TO authenticated
USING (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
);

-- =====================================================
-- Policy 3: Allow users to delete files from their organization folder
-- =====================================================
-- Users can DELETE files only from folders matching their organization_id

CREATE POLICY "Users can delete their organization files"
ON storage.objects 
FOR DELETE
TO authenticated
USING (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
);

-- =====================================================
-- Policy 4: Allow service role full access
-- =====================================================
-- Service role (backend API) has full access to all operations
-- This is used for administrative operations and bypasses user restrictions

CREATE POLICY "Service role has full access"
ON storage.objects
FOR ALL
TO service_role
USING (bucket_id = 'attachments')
WITH CHECK (bucket_id = 'attachments');

-- =====================================================
-- Optional: Additional policies for specific use cases
-- =====================================================

-- Policy 5: Allow users to update file metadata in their organization folder
-- Uncomment if you need users to update file metadata (e.g., rename files)

/*
CREATE POLICY "Users can update their organization file metadata"
ON storage.objects 
FOR UPDATE
TO authenticated
USING (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
)
WITH CHECK (
  bucket_id = 'attachments' AND
  (storage.foldername(name))[1] = auth.jwt() ->> 'organization_id'
);
*/

-- =====================================================
-- Verification Queries
-- =====================================================
-- Run these queries to verify policies are created correctly

-- List all policies for storage.objects
-- SELECT * FROM pg_policies WHERE tablename = 'objects' AND schemaname = 'storage';

-- Test policy for a specific user (replace with actual user JWT)
-- SET request.jwt.claims TO '{"organization_id": "123"}';
-- SELECT * FROM storage.objects WHERE bucket_id = 'attachments';

-- =====================================================
-- Notes
-- =====================================================
-- 
-- 1. These policies assume JWT tokens contain an 'organization_id' claim
-- 2. Folder structure must be: {organizationId}/{ticketId}/{filename}
-- 3. Service role bypasses all RLS policies
-- 4. Policies are enforced at the database level
-- 5. For local Supabase, run this script via Supabase Studio SQL Editor
-- 6. For production, include in migration scripts
--
-- =====================================================
