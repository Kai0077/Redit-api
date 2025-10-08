BEGIN;

TRUNCATE TABLE
    user_moderates_community,
  user_administrates_community,
  user_owns_community,
  user_communities,
  user_follows,
  community_moderators,
  community_admins,
  community_members,
  comments,
  post,
  community,
  "user"
RESTART IDENTITY CASCADE;

-- USERS (password = "admin1234", hashed with ASP.NET Core PasswordHasher)
INSERT INTO "user" (username, name, email, age, password_hash, aura, bio, profile_picture, account_status) VALUES
                                                                                                               ('kai',     'Kai Tsvetkov',    'kai@example.com',     22, 'AQAAAAIAAYagAAAAEPh4DT9yH9P5yY+Pp9l3Oq7YQOqZ4qL4PaOKL4rZUt0KkKi9QOBmVqYPrchZgV4qNQ==', 15, 'Full-stack dev and climber', NULL, 'online'),
                                                                                                               ('hanni',   'Hanni Lee',       'hanni@example.com',   24, 'AQAAAAIAAYagAAAAEPh4DT9yH9P5yY+Pp9l3Oq7YQOqZ4qL4PaOKL4rZUt0KkKi9QOBmVqYPrchZgV4qNQ==', 10, 'Frontend engineer',           NULL, 'idle'),
                                                                                                               ('wei',     'Wei Zhang',       'wei@example.com',     26, 'AQAAAAIAAYagAAAAEPh4DT9yH9P5yY+Pp9l3Oq7YQOqZ4qL4PaOKL4rZUt0KkKi9QOBmVqYPrchZgV4qNQ==',  5, 'Database wizard',             NULL, 'offline'),
                                                                                                               ('mikkel',  'Mikkel Sørensen', 'mikkel@example.com',  25, 'AQAAAAIAAYagAAAAEPh4DT9yH9P5yY+Pp9l3Oq7YQOqZ4qL4PaOKL4rZUt0KkKi9QOBmVqYPrchZgV4qNQ==',  8, 'Sysadmin & hobby drummer',    NULL, 'do_not_disturb');

-- COMMUNITIES
INSERT INTO community (name, description, profile_picture, owner_username, pinned_post_ids) VALUES
                                                                                                ('climbing', 'All about bouldering and routesetting', NULL, 'kai', ARRAY[]::int[]),
                                                                                                ('devs',     'Programming discussions and code review', NULL, 'hanni', ARRAY[]::int[]);

-- POSTS
INSERT INTO post (id, title, description, aura, original_poster, community, embeds, status) VALUES
                                                                                                (1, 'Best shoes for overhangs?', 'Looking for suggestions for steep boulders.', 12, 'kai', 'climbing', ARRAY['https://example.com/shoe1'], 'active'),
                                                                                                (2, 'Show your dotnet tips',     'Share small ASP.NET Core snippets.',          20, 'hanni', 'devs', ARRAY['https://gist.github.com/abc123'], 'active'),
                                                                                                (3, 'Hand care routine',         'How do you tape and recover skin?',            7, 'wei', 'climbing', ARRAY[]::text[], 'archived');

UPDATE community SET pinned_post_ids = ARRAY[1] WHERE name = 'climbing';
UPDATE community SET pinned_post_ids = ARRAY[2] WHERE name = 'devs';

-- COMMENTS
INSERT INTO comments (id, text, embeds, aura, commenter, post_id, parent_id) VALUES
                                                                                 (1, 'I like aggressive toe boxes for overhangs.', ARRAY[]::text[], 5, 'hanni', 1, NULL),
                                                                                 (2, 'Seconding that. Try the Scarpa Drago.',      ARRAY[]::text[], 3, 'wei',   1, 1),
                                                                                 (3, 'Use Minimal APIs for lightweight endpoints.',ARRAY[]::text[], 9, 'kai',   2, NULL),
                                                                                 (4, 'Taping tutorial: https://example.com/tape',  ARRAY['https://example.com/tape'], 2, 'mikkel', 3, NULL);

-- COMMUNITY MEMBERS
INSERT INTO community_members (community_name, username) VALUES
                                                             ('climbing', 'kai'),
                                                             ('climbing', 'wei'),
                                                             ('climbing', 'mikkel'),
                                                             ('devs',     'hanni'),
                                                             ('devs',     'kai');

-- COMMUNITY ADMINS
INSERT INTO community_admins (community_name, username) VALUES
                                                            ('climbing', 'kai'),
                                                            ('devs',     'hanni');

-- COMMUNITY MODERATORS
INSERT INTO community_moderators (community_name, username) VALUES
                                                                ('climbing', 'wei'),
                                                                ('devs',     'mikkel');

-- USER FOLLOWS
INSERT INTO user_follows (follower_username, following_username) VALUES
                                                                     ('kai', 'hanni'),
                                                                     ('hanni', 'wei'),
                                                                     ('wei', 'mikkel'),
                                                                     ('mikkel', 'kai');

-- USER COMMUNITIES
INSERT INTO user_communities (username, community_name) VALUES
                                                            ('kai', 'climbing'),
                                                            ('kai', 'devs'),
                                                            ('hanni', 'devs'),
                                                            ('wei', 'climbing'),
                                                            ('mikkel', 'climbing');

-- USER OWNS / ADMINISTRATES / MODERATES COMMUNITIES
INSERT INTO user_owns_community (username, community_name) VALUES
                                                               ('kai', 'climbing'),
                                                               ('hanni', 'devs');

INSERT INTO user_administrates_community (username, community_name) VALUES
                                                                        ('kai', 'climbing'),
                                                                        ('hanni', 'devs');

INSERT INTO user_moderates_community (username, community_name) VALUES
                                                                    ('wei', 'climbing'),
                                                                    ('mikkel', 'devs');

-- Reset SERIAL sequences
SELECT setval(pg_get_serial_sequence('post','id'),     (SELECT COALESCE(MAX(id),1) FROM post),     true);
SELECT setval(pg_get_serial_sequence('comments','id'), (SELECT COALESCE(MAX(id),1) FROM comments), true);

COMMIT;