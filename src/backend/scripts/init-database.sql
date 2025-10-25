-- FLOWerTRACK Database Initialization Script
-- This script creates the database and user for local development

-- Create database
CREATE DATABASE flowertrack_dev;

-- Create user (if needed)
-- CREATE USER flowertrack_user WITH PASSWORD 'your_secure_password';

-- Grant privileges
-- GRANT ALL PRIVILEGES ON DATABASE flowertrack_dev TO flowertrack_user;

-- Connect to the database
\c flowertrack_dev;

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
