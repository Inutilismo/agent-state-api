-- PostgreSQL database initialization
-- Script executed automatically when container starts

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

DO $$
BEGIN
    RAISE NOTICE 'AgentState Database initialized successfully!';
    RAISE NOTICE 'Ready to accept connections from the API.';
END $$;
