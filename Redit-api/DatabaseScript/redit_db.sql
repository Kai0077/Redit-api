-- Create database
CREATE
DATABASE redit_db;
\c
redit_db;

-- ENUM types
CREATE TYPE user_status AS ENUM ('online', 'idle', 'offline', 'invisible', 'dnd');
CREATE TYPE post_status AS ENUM ('active', 'archived');

-- USER table
CREATE TABLE "User"
(
    name     VARCHAR(100)                   NOT NULL,
    username VARCHAR(50) UNIQUE PRIMARY KEY NOT NULL,
    mail     VARCHAR(150) UNIQUE            NOT NULL,
    age      INT CHECK (age >= 13),
    pswdHash VARCHAR(255)                   NOT NULL,
    aura     INT         DEFAULT 0,
    bio      TEXT,
    pfp      TEXT,
    status   user_status DEFAULT 'offline'
);

-- COMMUNITY table
CREATE TABLE Community
(
    name           VARCHAR(100) UNIQUE PRIMARY KEY NOT NULL,
    description    TEXT,
    pfp            TEXT,
    owner_username VARCHAR(50)                     REFERENCES "User" (username) ON DELETE SET NULL,
    pinned         INT[] DEFAULT '{}'
);

-- POST table
CREATE TABLE Post
(
    id          SERIAL PRIMARY KEY,
    title       VARCHAR(200) NOT NULL,
    description TEXT,
    aura        INT         DEFAULT 0,
    owner       INT REFERENCES "User" (username) ON DELETE CASCADE,
    community   VARCHAR(100) REFERENCES Community (name) ON DELETE CASCADE,
    embeds      TEXT[] DEFAULT '{}',
    status      post_status DEFAULT 'active'
);

-- COMMENT table
CREATE TABLE Comment
(
    id        SERIAL PRIMARY KEY,
    text      TEXT NOT NULL,
    embeds    TEXT[] DEFAULT '{}',
    aura      INT DEFAULT 0,
    username  INT REFERENCES "User" (username) ON DELETE CASCADE,
    post_id   INT REFERENCES Post (id) ON DELETE CASCADE,
    parent_id INT REFERENCES Comment (id) ON DELETE CASCADE
);

-- RELATIONSHIPS (many-to-many)
-- Community members
CREATE TABLE CommunityMembers
(
    community VARCHAR(100) REFERENCES Community (name) ON DELETE CASCADE,
    username  VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    PRIMARY KEY (name, username)
);

-- Community admins
CREATE TABLE CommunityAdmins
(
    community VARCHAR(100) REFERENCES Community (name) ON DELETE CASCADE,
    username  VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    PRIMARY KEY (name, username)
);

-- Community moderators
CREATE TABLE CommunityMods
(
    community VARCHAR(100) REFERENCES Community (name) ON DELETE CASCADE,
    username  VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    PRIMARY KEY (name, username)
);

-- User follows (self-referencing many-to-many)
CREATE TABLE UserFollows
(
    followers VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    following VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    PRIMARY KEY (followers, following)
);

-- User communities (subscriptions)
CREATE TABLE UserCommunities
(
    username  VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    community INT REFERENCES Community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community)
);

-- User owns/admins/moderates communities
CREATE TABLE UserOwnsCommunity
(
    username  VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    community VARCHAR(100) REFERENCES Community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community)
);

CREATE TABLE UserAdministratesCommunity
(
    username  VARCHAR(50) REFERENCES "User" (username) ON DELETE CASCADE,
    community VARCHAR(100) REFERENCES Community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community)
);

CREATE TABLE UserModeratesCommunity
(
    username  INT REFERENCES "User" (username) ON DELETE CASCADE,
    community INT REFERENCES Community (name) ON DELETE CASCADE,
    PRIMARY KEY (username, community)
);

-- OPTIONAL: Comments can have replies (self-reference)
ALTER TABLE Comment
    ADD CONSTRAINT fk_comment_parent
        FOREIGN KEY (parent_id) REFERENCES Comment (id) ON DELETE CASCADE;
