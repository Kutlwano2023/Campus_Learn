-- Database: campus_learn_db

-- DROP DATABASE IF EXISTS campus_learn_db;

CREATE DATABASE campus_learn_db
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'English_United States.1252'
    LC_CTYPE = 'English_United States.1252'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;


-- Create ChatConversations table
CREATE TABLE IF NOT EXISTS "ChatConversations" (
    "ConversationId" uuid PRIMARY KEY,
    "UserId" text NOT NULL,
    "Title" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "ConversationType" character varying(50) DEFAULT 'General',
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Create LearningMaterials table
CREATE TABLE IF NOT EXISTS "LearningMaterials" (
    "MaterialID" serial PRIMARY KEY,
    "Title" character varying(200) NOT NULL,
    "MaterialType" character varying(50),
    "FilePathURL" character varying(500)
);

-- Create Tutors table
CREATE TABLE IF NOT EXISTS "Tutors" (
    "TutorID" serial PRIMARY KEY,
    "FirstName" character varying(100),
    "LastName" character varying(100),
    "Specialization" character varying(200),
    "UserId" text,
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE SET NULL
);

-- Create ChatMessages table (after ChatConversations due to foreign key)
CREATE TABLE IF NOT EXISTS "ChatMessages" (
    "MessageId" uuid PRIMARY KEY,
    "ConversationId" uuid NOT NULL,
    "UserId" text,
    "MessageText" text NOT NULL,
    "IsUserMessage" boolean NOT NULL,
    "MessageType" character varying(50) DEFAULT 'Text',
    "CreatedAt" timestamp with time zone NOT NULL,
    "Metadata" text,
    FOREIGN KEY ("ConversationId") REFERENCES "ChatConversations" ("ConversationId") ON DELETE CASCADE,
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE SET NULL
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_ChatConversations_UserId" ON "ChatConversations" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_ChatMessages_ConversationId" ON "ChatMessages" ("ConversationId");
CREATE INDEX IF NOT EXISTS "IX_ChatMessages_UserId" ON "ChatMessages" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_Tutors_UserId" ON "Tutors" ("UserId");