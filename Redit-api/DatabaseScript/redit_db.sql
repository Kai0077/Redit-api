DROP DATABASE IF EXISTS redit_db;

CREATE DATABASE redit_db;

-- ENUM types
CREATE TYPE user_status AS ENUM ('online', 'idle', 'offline', 'invisible', 'do_not_disturb');
CREATE TYPE post_status AS ENUM ('active', 'archived');

-- NEW: role enum
CREATE TYPE user_role AS ENUM ('user', 'super_user');

-- USERS
CREATE TABLE "user"
(
    username        VARCHAR(50) PRIMARY KEY,
    name            VARCHAR(100)        NOT NULL,
    email           VARCHAR(150) UNIQUE NOT NULL,
    age             INT CHECK (age >= 13),
    password_hash   VARCHAR(255)        NOT NULL,
    aura            INT         DEFAULT 0,
    bio             TEXT,
    profile_picture TEXT,
    account_status  user_status DEFAULT 'offline',
    role            user_role   NOT NULL DEFAULT 'user'   -- NEW COLUMN
);

-- COMMUNITIES
CREATE TABLE community
(
    name            VARCHAR(100) PRIMARY KEY,
    description     TEXT,
    profile_picture TEXT,
    owner_username  VARCHAR(50) REFERENCES "user" (username) ON DELETE SET NULL,
    pinned_post_ids INT[] DEFAULT '{}'
);

-- POSTS
CREATE TABLE post
(
    id              SERIAL PRIMARY KEY,
    title           VARCHAR(200) NOT NULL,
    description     TEXT,
    aura            INT         DEFAULT 0,
    original_poster VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    community       VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    embeds          TEXT[]      DEFAULT '{}',
    status          post_status DEFAULT 'active'
);

-- COMMENTS
CREATE TABLE comments
(
    id        SERIAL PRIMARY KEY,
    text      TEXT NOT NULL,
    embeds    TEXT[] DEFAULT '{}',
    aura      INT    DEFAULT 0,
    commenter VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    post_id   INT REFERENCES post (id) ON DELETE CASCADE,
    parent_id INT REFERENCES comments (id) ON DELETE CASCADE
);

-- COMMUNITY MEMBERS (many-to-many)
CREATE TABLE community_members
(
    community_name VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    username       VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    PRIMARY KEY (community_name, username)
);

-- COMMUNITY ADMINS
CREATE TABLE community_admins
(
    community_name VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    username       VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    PRIMARY KEY (community_name, username)
);

-- COMMUNITY MODERATORS
CREATE TABLE community_moderators
(
    community_name VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    username       VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    PRIMARY KEY (community_name, username)
);

-- USER FOLLOWS (self-referencing many-to-many)
CREATE TABLE user_follows
(
    follower_username  VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    following_username VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    PRIMARY KEY (follower_username, following_username),
    CHECK (follower_username <> following_username)
);

-- USER COMMUNITIES (subscriptions)
CREATE TABLE user_communities
(
    username       VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    community_name VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community_name)
);

-- USER OWNS / ADMINISTRATES / MODERATES COMMUNITIES
CREATE TABLE user_owns_community
(
    username       VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    community_name VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community_name)
);

CREATE TABLE user_administrates_community
(
    username       VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    community_name VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community_name)
);

CREATE TABLE user_moderates_community
(
    username       VARCHAR(50) REFERENCES "user" (username) ON DELETE CASCADE,
    community_name VARCHAR(100) REFERENCES community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community_name)
);

-- Helpful indexes
CREATE INDEX idx_posts_community ON post (community);
CREATE INDEX idx_posts_poster   ON post (original_poster);
CREATE INDEX idx_comments_post  ON comments (post_id);
CREATE INDEX idx_comments_parent ON comments (parent_id);

-- OPTIONAL: promote specific users to super_user later (adjust the list as you wish)
-- UPDATE "user"
-- SET role = 'super_user'
-- WHERE username IN ('kai', 'simon', 'anna', 'maria');