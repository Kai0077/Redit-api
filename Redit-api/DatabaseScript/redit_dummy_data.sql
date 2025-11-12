BEGIN;

TRUNCATE TABLE
    user_moderates_community,
  user_administrates_community,
  user_owns_community,
  user_communities,
  community_moderators,
  community_admins,
  community_members,
  comments,
  post,
  community,
  "user"
RESTART IDENTITY CASCADE;

-- USERS (password = "admin1234", ASP.NET Core PasswordHasher hash)
-- NOTE: includes role column now
INSERT INTO "user"
(username, name, email, age, password_hash, aura, bio, profile_picture, account_status, role)
VALUES
    ('hans',     'Hans Hansen',    'hans@example.com',     22, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 15, 'Full-stack dev and climber', NULL, 'online',          'super_user'),
    ('sten',   'Sten Bob',       'hanni@example.com',   24, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 10, 'Frontend engineer',           NULL, 'idle',            'super_user'),
    ('alice',     'Alice alice',       'wei@example.com',     26, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==',  5, 'Database wizard',             NULL, 'offline',         'super_user'),
    ('mikkel',  'Mikkel Sørensen', 'mikkel@example.com',  25, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==',  8, 'Sysadmin & hobby drummer',    NULL, 'do_not_disturb',  'super_user'),
    ('weiadmin', 'Wei Yang', 'wei@gmail.com', 23, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 10, NULL, NULL, 'offline', 'super_user'),
    ('kaiadmin', 'Kai Tsvetkov', 'kai@gmail.com', 23, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 10, NULL, NULL, 'offline', 'super_user'),
    ('hanniadmin', 'Hanni Lee', 'hanni@gmail.com', 23, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 10, NULL, NULL, 'offline', 'super_user'),
    ('mikkeladmin', 'Mikkel Hansen', 'mikkel@gmail.com', 23, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 10, NULL, NULL, 'offline', 'super_user'),
    ('bob', 'Bob Sten', 'bob@outlook.com', 23, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 10, NULL, NULL, 'offline', 'user'),
    ('søren', 'Søren Sten', 'søren@gmail.com', 23, 'AQAAAAIAAYagAAAAEKrQuJlCKbOgN7szTQ1LsHPxjbGiu+u7Yib9+50rDUuawwPuLBLuo9Ai0BlF+co/2A==', 10, NULL, NULL, 'offline', 'super_user');

-- COMMUNITIES
INSERT INTO community (name, description, profile_picture, owner_username, pinned_post_ids) 
VALUES
    ('climbing', 'All about bouldering and routesetting', NULL, 'kaiadmin',   ARRAY[]::int[]),
    ('devs',     'Programming discussions and code review', NULL, 'hanniadmin', ARRAY[]::int[]),
    ('gaming',   'Talk about games, releases, and reviews', NULL, 'bob', ARRAY[]::int[]),
    ('music',    'Music production, sharing, and critique', NULL, 'søren', ARRAY[]::int[]),
    ('books',    'Book discussions, reviews, and recommendations', NULL, 'mikkel', ARRAY[]::int[]),
    ('design',   'UI/UX discussions and feedback', NULL, 'alice', ARRAY[]::int[]),
    ('hardware', 'PC builds, peripherals, and performance tuning', NULL, 'sten', ARRAY[]::int[]),
    ('startups', 'Entrepreneurship, pitching, and product ideas', NULL, 'hans', ARRAY[]::int[]),
    ('ai',       'AI research, models, and prompt engineering', NULL, 'alice', ARRAY[]::int[]),
    ('movies',   'Film analysis, reviews, and recommendations', NULL, 'alice', ARRAY[]::int[]);
                                                                                                

-- POSTS
INSERT INTO post (id, title, description, aura, original_poster, community, embeds, status, is_public, publish_at) 
VALUES
    (1, 'New overhang project', 'Just started working on a 7A roof problem, open to beta advice!', 12, 'hans', 'climbing', ARRAY['https://example.com/overhang1'], 'active', True, NULL),
    (2, 'Favorite IDE for C#', 'What IDE do you prefer for .NET development?', 22, 'hanniadmin', 'devs', ARRAY[]::text[], 'active', True, NULL),
    (3, 'Best boss fights ever', 'What are your most memorable boss fights in games?', 15, 'bob', 'gaming', ARRAY['https://example.com/bossfight1'], 'active', True, NULL),
    (4, 'Mixing vocals', 'How do you make vocals cut through the mix without harshness?', 9, 'søren', 'music', ARRAY['https://example.com/mix1'], 'active', True, NULL),
    (5, 'Top 2025 reads', 'What’s your favorite book you read this year?', 17, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (6, 'Minimalist UI inspirations', 'Share examples of elegant minimalist designs.', 20, 'alice', 'design', ARRAY['https://example.com/ui1'], 'active', True, NULL),
    (7, 'Building a quiet PC', 'Any tips for silent cooling setups?', 14, 'sten', 'hardware', ARRAY['https://example.com/quietpc1'], 'active', True, NULL),
    (8, 'Pitch deck advice', 'How do you structure a good early-stage investor deck?', 25, 'hans', 'startups', ARRAY[]::text[], 'active', True, NULL),
    (9, 'Prompt engineering tricks', 'Best practices for working with GPT models?', 30, 'weiadmin', 'ai', ARRAY['https://example.com/prompt1'], 'active', True, NULL),
    (10, 'Favorite film soundtracks', 'What movie soundtrack always sticks with you?', 11, 'alice', 'movies', ARRAY['https://example.com/soundtrack1'], 'active', True, NULL),

    (11, 'Footwork drills for slabs', 'How to stay balanced on small footholds?', 8, 'kaiadmin', 'climbing', ARRAY[]::text[], 'active', True, NULL),
    (12, 'Optimizing Entity Framework', 'How do you reduce query overhead in EF Core?', 21, 'mikkeladmin', 'devs', ARRAY['https://gist.github.com/efopt'], 'active', True, NULL),
    (13, 'Best open-world games', 'Looking for story-rich open world experiences.', 16, 'bob', 'gaming', ARRAY[]::text[], 'active', True, NULL),
    (14, 'Best studio monitors under $300', 'Looking for solid nearfields for mixing.', 10, 'søren', 'music', ARRAY['https://example.com/monitors1'], 'archived', True, NULL),
    (15, 'Sci-fi book recommendations', 'Prefer hard science fiction with deep lore.', 9, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (16, 'Color palettes that pop', 'What’s your go-to contrast combo?', 13, 'alice', 'design', ARRAY['https://example.com/colors1'], 'active', True, NULL),
    (17, 'Mechanical keyboard builds', 'Post your favorite builds and switches.', 18, 'sten', 'hardware', ARRAY[]::text[], 'active', True, NULL),
    (18, 'Naming your startup', 'How did you come up with your company name?', 22, 'hans', 'startups', ARRAY['https://example.com/name1'], 'active', True, NULL),
    (19, 'AI-generated art ethics', 'Where do you draw the line with AI tools?', 19, 'weiadmin', 'ai', ARRAY['https://example.com/aiart1'], 'active', True, NULL),
    (20, 'Favorite cinematography', 'Movies that impressed you visually.', 8, 'alice', 'movies', ARRAY[]::text[], 'active', True, NULL),

    (21, 'Crimp training tips', 'Looking for ways to strengthen fingers safely.', 11, 'søren', 'climbing', ARRAY['https://example.com/crimp1'], 'active', True, NULL),
    (22, 'Best VS Code extensions', 'Share your must-have productivity add-ons.', 25, 'hanniadmin', 'devs', ARRAY[]::text[], 'active', True, NULL),
    (23, 'Retro gaming nostalgia', 'What console defined your childhood?', 17, 'bob', 'gaming', ARRAY['https://example.com/retro1'], 'active', True, NULL),
    (24, 'DAW workflow tips', 'How do you organize your music projects efficiently?', 10, 'søren', 'music', ARRAY['https://example.com/daw1'], 'active', True, NULL),
    (25, 'Fantasy novels with deep worldbuilding', 'Need new fantasy reads.', 12, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (26, 'Typography that inspires', 'What’s your favorite typeface combo?', 15, 'alice', 'design', ARRAY['https://example.com/type1'], 'active', True, NULL),
    (27, 'Best GPUs for developers', 'What GPU do you use for AI or rendering?', 14, 'sten', 'hardware', ARRAY[]::text[], 'archived', True, NULL),
    (28, 'Fundraising pitfalls', 'What are some mistakes you made early on?', 18, 'hans', 'startups', ARRAY['https://example.com/fund1'], 'active', True, NULL),
    (29, 'Fine-tuning models', 'How do you approach small dataset fine-tuning?', 26, 'weiadmin', 'ai', ARRAY[]::text[], 'active', True, NULL),
    (30, 'Hidden gem films', 'Movies you think deserve more attention.', 10, 'alice', 'movies', ARRAY['https://example.com/gem1'], 'active', True, NULL),

    (31, 'Dyno beta help', 'Can’t stick a move on my project—need advice.', 7, 'bob', 'climbing', ARRAY['https://example.com/dyno1'], 'active', True, NULL),
    (32, 'Refactoring .NET APIs', 'Best way to clean legacy controllers.', 16, 'hanniadmin', 'devs', ARRAY[]::text[], 'active', True, NULL),
    (33, 'Esports predictions 2026', 'Who do you think will dominate next year?', 23, 'bob', 'gaming', ARRAY[]::text[], 'active', True, NULL),
    (34, 'Mastering EQ cuts', 'Where do you usually cut frequencies for vocals?', 15, 'søren', 'music', ARRAY[]::text[], 'archived', True, NULL),
    (35, 'Classic literature thread', 'Let’s talk about timeless novels.', 14, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (36, 'Redesign feedback thread', 'Drop your designs for peer review!', 19, 'alice', 'design', ARRAY['https://example.com/ux1'], 'active', True, NULL),
    (37, 'SSD recommendations', 'What drives are reliable for builds?', 20, 'sten', 'hardware', ARRAY['https://example.com/ssd1'], 'active', True, NULL),
    (38, 'Pitch feedback request', 'Would love input on my MVP presentation.', 22, 'hans', 'startups', ARRAY[]::text[], 'active', True, NULL),
    (39, 'Prompt chaining experiments', 'Anyone tried chaining LLMs for reasoning?', 28, 'weiadmin', 'ai', ARRAY['https://example.com/chain1'], 'active', True, NULL),
    (40, 'Underrated directors', 'Filmmakers you think deserve more love.', 11, 'alice', 'movies', ARRAY[]::text[], 'active', True, NULL),

    (41, 'Hangboard progress log', 'Tracking small gains over months.', 15, 'hans', 'climbing', ARRAY['https://example.com/hangboard1'], 'archived', True, NULL),
    (42, 'Async vs threading in C#', 'When to use one over the other?', 27, 'mikkeladmin', 'devs', ARRAY['https://gist.github.com/async1'], 'active', True, NULL),
    (43, 'VR gaming setups', 'What’s the best affordable VR rig?', 13, 'bob', 'gaming', ARRAY['https://example.com/vr1'], 'active', True, NULL),
    (44, 'Mix bus compression', 'Do you compress the master bus early?', 14, 'søren', 'music', ARRAY[]::text[], 'active', True, NULL),
    (45, 'Cozy reads for rainy days', 'Books that feel like a blanket.', 9, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (46, 'Logo critique thread', 'Drop your latest logo for feedback.', 16, 'alice', 'design', ARRAY['https://example.com/logo1'], 'active', True, NULL),
    (47, 'CPU cooling experiments', 'Anyone tried liquid metal?', 18, 'sten', 'hardware', ARRAY[]::text[], 'active', True, NULL),
    (48, 'Startup naming poll', 'Would you prefer short or abstract names?', 11, 'hans', 'startups', ARRAY[]::text[], 'active', True, NULL),
    (49, 'GPT agent frameworks', 'What’s the easiest framework to start with?', 29, 'weiadmin', 'ai', ARRAY['https://example.com/agent1'], 'active', True, NULL),
    (50, 'Sound design in horror films', 'Favorite sound tricks in horror?', 10, 'alice', 'movies', ARRAY[]::text[], 'active', True, NULL),

    (51, 'Compression technique tips', 'How to avoid overcompressing drums.', 12, 'søren', 'music', ARRAY[]::text[], 'active', True, NULL),
    (52, 'Improving crimp strength safely', 'Any structured fingerboard plans?', 17, 'mikkel', 'climbing', ARRAY['https://example.com/fingerplan'], 'active', True, NULL),
    (53, 'AI for code completion', 'Who’s using Copilot daily?', 25, 'hanniadmin', 'devs', ARRAY['https://example.com/copilot'], 'active', True, NULL),
    (54, 'Portable gaming consoles', 'Steam Deck vs ROG Ally?', 21, 'bob', 'gaming', ARRAY[]::text[], 'active', True, NULL),
    (55, 'Character development tips', 'How to write believable arcs?', 15, 'mikkel', 'books', ARRAY[]::text[], 'archived', True, NULL),
    (56, 'Dark mode UX challenges', 'What’s your approach to contrast?', 19, 'alice', 'design', ARRAY['https://example.com/darkmode'], 'active', True, NULL),
    (57, 'Overclocking results', 'Share your stable overclocks!', 20, 'sten', 'hardware', ARRAY['https://example.com/oc1'], 'active', True, NULL),
    (58, 'Bootstrapping stories', 'Any founders who avoided VC funding?', 23, 'hans', 'startups', ARRAY[]::text[], 'active', True, NULL),
    (59, 'Transformer visualization', 'Cool ways to visualize model attention.', 27, 'weiadmin', 'ai', ARRAY['https://example.com/attention'], 'active', True, NULL),
    (60, 'Cinematography lighting setups', 'Share lighting breakdowns.', 9, 'alice', 'movies', ARRAY[]::text[], 'active', True, NULL),

    (61, 'Beta for new gym route', 'Tall dyno to a jug—need advice.', 11, 'kaiadmin', 'climbing', ARRAY['https://example.com/dyno2'], 'active', True, NULL),
    (62, 'ASP.NET Middleware tricks', 'Hidden gems most devs overlook.', 21, 'mikkeladmin', 'devs', ARRAY['https://gist.github.com/mw1'], 'active', True, NULL),
    (63, 'Best rhythm games', 'What rhythm game has the tightest timing?', 17, 'bob', 'gaming', ARRAY['https://example.com/rhythm'], 'active', True, NULL),
    (64, 'Mastering workflow order', 'EQ or compression first?', 8, 'søren', 'music', ARRAY[]::text[], 'archived', True, NULL),
    (65, 'Book-to-film adaptations', 'Which ones nailed it?', 16, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (66, 'Portfolio feedback', 'Drop your Dribbble or Behance link.', 14, 'alice', 'design', ARRAY['https://example.com/portfolio'], 'active', True, NULL),
    (67, 'Favorite PC cases', 'Best airflow designs right now?', 18, 'sten', 'hardware', ARRAY[]::text[], 'active', True, NULL),
    (68, 'Pitch deck critique', 'Looking for real investor feedback.', 22, 'hans', 'startups', ARRAY[]::text[], 'active', True, NULL),
    (69, 'LLM prompt playground', 'Share your weirdest successful prompts.', 26, 'weiadmin', 'ai', ARRAY['https://example.com/prompt2'], 'active', True, NULL),
    (70, 'Best animated films', 'Which animation blew your mind visually?', 12, 'alice', 'movies', ARRAY['https://example.com/animated'], 'active', True, NULL),

    (71, 'Chalk brand comparisons', 'Liquid vs block—your pick?', 9, 'søren', 'climbing', ARRAY[]::text[], 'active', True, NULL),
    (72, 'Docker in .NET projects', 'Simplifying microservice deployments.', 24, 'hanniadmin', 'devs', ARRAY['https://gist.github.com/docker2'], 'active', True, NULL),
    (73, 'LAN party nostalgia', 'Miss the 2000s LAN days?', 18, 'bob', 'gaming', ARRAY[]::text[], 'archived', True, NULL),
    (74, 'Layering instruments', 'How do you make arrangements sound full?', 13, 'søren', 'music', ARRAY['https://example.com/arrangement'], 'active', True, NULL),
    (75, 'Books that changed your life', 'Let’s talk about transformative reads.', 20, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (76, 'Figma auto-layout hacks', 'Share cool layout tips.', 19, 'alice', 'design', ARRAY['https://example.com/figma'], 'active', True, NULL),
    (77, 'Best PSUs for silent builds', 'Which brands have low coil whine?', 15, 'sten', 'hardware', ARRAY[]::text[], 'active', True, NULL),
    (78, 'Founders burnout talk', 'How do you manage stress?', 18, 'hans', 'startups', ARRAY['https://example.com/burnout'], 'active', True, NULL),
    (79, 'AI music generation', 'Tools that actually sound decent?', 25, 'weiadmin', 'ai', ARRAY['https://example.com/aimusic'], 'active', True, NULL),
    (80, 'Movie color grading', 'Favorite LUTs or looks?', 11, 'alice', 'movies', ARRAY['https://example.com/grading'], 'active', True, NULL),

    (81, 'Crack climbing tape tips', 'How do you tape for jams properly?', 6, 'hans', 'climbing', ARRAY['https://example.com/crack2'], 'active', True, NULL),
    (82, 'C# testing best practices', 'What’s your test pyramid look like?', 23, 'hanniadmin', 'devs', ARRAY['https://gist.github.com/tests'], 'active', True, NULL),
    (83, 'Upcoming indie games', 'Hidden gems for 2026?', 19, 'bob', 'gaming', ARRAY['https://example.com/indie'], 'active', True, NULL),
    (84, 'Mix translation issues', 'How to make mixes sound good everywhere?', 14, 'søren', 'music', ARRAY[]::text[], 'active', True, NULL),
    (85, 'Co-op book writing', 'Has anyone written collaboratively?', 10, 'mikkel', 'books', ARRAY[]::text[], 'archived', True, NULL),
    (86, 'Animation in web UI', 'Microinteractions that delight.', 18, 'alice', 'design', ARRAY['https://example.com/animation'], 'active', True, NULL),
    (87, 'RAM overclocking results', 'Any benefit in real workloads?', 15, 'sten', 'hardware', ARRAY[]::text[], 'active', True, NULL),
    (88, 'MVP validation methods', 'How do you test early demand?', 19, 'hans', 'startups', ARRAY['https://example.com/validation'], 'active', True, NULL),
    (89, 'LLM hallucination fixes', 'How to reduce factual errors?', 30, 'weiadmin', 'ai', ARRAY['https://example.com/hallucination'], 'active', True, NULL),
    (90, 'Top cinematography trends 2025', 'What styles are becoming popular?', 14, 'alice', 'movies', ARRAY[]::text[], 'active', True, NULL),

    (91, 'Campus board intervals', 'Best rest timing for power training.', 8, 'kaiadmin', 'climbing', ARRAY['https://example.com/campus'], 'active', True, NULL),
    (92, 'Dependency injection tips', 'Cleaner startup configurations.', 24, 'mikkeladmin', 'devs', ARRAY['https://gist.github.com/di'], 'active', True, NULL),
    (93, 'Controller deadzones', 'What settings do you use?', 17, 'bob', 'gaming', ARRAY['https://example.com/controller'], 'active', True, NULL),
    (94, 'Vocal compression chain', 'Your go-to compressor plugin?', 12, 'søren', 'music', ARRAY[]::text[], 'active', True, NULL),
    (95, 'Book covers you love', 'Which ones caught your eye instantly?', 16, 'mikkel', 'books', ARRAY[]::text[], 'active', True, NULL),
    (96, 'Accessibility in design', 'How do you ensure accessibility compliance?', 21, 'alice', 'design', ARRAY['https://example.com/a11y'], 'active', True, NULL),
    (97, 'PC cable management', 'How to keep everything neat?', 18, 'sten', 'hardware', ARRAY[]::text[], 'active', True, NULL),
    (98, 'Exit strategy advice', 'When’s the right time to sell?', 19, 'hans', 'startups', ARRAY['https://example.com/exit'], 'active', True, NULL),
    (99, 'AI text-to-3D experiments', 'Anyone playing with 3D gen models?', 27, 'weiadmin', 'ai', ARRAY['https://example.com/text3d'], 'active', True, NULL),
    (100, 'Best movie endings', 'Which finales left you speechless?', 13, 'alice', 'movies', ARRAY[]::text[], 'active', True, NULL);

UPDATE community SET pinned_post_ids = ARRAY[1] WHERE name = 'climbing';
UPDATE community SET pinned_post_ids = ARRAY[2] WHERE name = 'devs';

-- COMMENTS
INSERT INTO comments (id, text, embeds, aura, commenter, post_id, parent_id) 
VALUES
    (1, 'Nice progress! Overhangs are brutal but rewarding.', ARRAY[]::text[], 6, 'alice', 1, NULL),
    (2, 'Try focusing on core tension—helps a lot on roofs.', ARRAY[]::text[], 4, 'søren', 1, 1),
    (3, 'VS Code + JetBrains Rider combo works great for .NET.', ARRAY[]::text[], 8, 'hanniadmin', 2, NULL),
    (4, 'Totally agree. Rider’s refactoring tools are amazing.', ARRAY[]::text[], 5, 'weiadmin', 2, 3),
    (5, 'Great callout—boss fights in Hollow Knight were peak design.', ARRAY[]::text[], 7, 'bob', 3, NULL),
    (6, 'I still think Sekiro had the best combat system.', ARRAY[]::text[], 4, 'mikkel', 3, 5),
    (7, 'Love this! The color contrast really makes the UI pop.', ARRAY[]::text[], 9, 'alice', 6, NULL),
    (8, 'You could also try a softer palette for accessibility.', ARRAY[]::text[], 5, 'sten', 6, 7),
    (9, 'That prompt chaining idea sounds super useful!', ARRAY['https://example.com/chain-example'], 10, 'kaiadmin', 39, NULL),
    (10, 'I tested a similar setup—it’s surprisingly accurate.', ARRAY[]::text[], 7, 'weiadmin', 39, 9);

-- COMMUNITY MEMBERS
INSERT INTO community_members (community_name, username) 
VALUES
    ('gaming',   'bob'),
    ('gaming',   'hanniadmin'),
    ('music',    'søren'),
    ('music',    'alice'),
    ('books',    'mikkel'),
    ('design',   'alice'),
    ('hardware', 'sten'),
    ('startups', 'hans'),
    ('ai',       'weiadmin'),
    ('movies',   'mikkeladmin');

-- COMMUNITY ADMINS
INSERT INTO community_admins (community_name, username) 
VALUES
    ('climbing', 'kaiadmin'),
    ('devs',     'hanniadmin'),
    ('gaming',   'bob'),
    ('music',    'søren'),
    ('books',    'mikkel'),
    ('design',   'alice'),
    ('hardware', 'sten'),
    ('startups', 'hans'),
    ('ai',       'weiadmin'),
    ('movies',   'mikkeladmin');

-- COMMUNITY MODERATORS
INSERT INTO community_moderators (community_name, username) 
VALUES
    ('climbing', 'weiadmin'),
    ('devs',     'mikkel'),
    ('gaming',   'bob'),
    ('music',    'søren'),
    ('books',    'alice'),
    ('design',   'hanniadmin'),
    ('hardware', 'sten'),
    ('startups', 'kaiadmin'),
    ('ai',       'mikkeladmin'),
    ('movies',   'hans');

-- USER COMMUNITIES
INSERT INTO user_communities (username, community_name) 
VALUES
    ('kaiadmin',    'climbing'),
    ('kaiadmin',    'devs'),
    ('hanniadmin',  'devs'),
    ('weiadmin',    'ai'),
    ('mikkel',      'books'),
    ('søren',       'music'),
    ('alice',       'design'),
    ('bob',         'gaming'),
    ('sten',        'hardware'),
    ('hans',        'startups');

-- USER OWNS / ADMINISTRATES / MODERATES COMMUNITIES
INSERT INTO user_owns_community (username, community_name) 
VALUES
    ('kaiadmin',    'climbing'),
    ('hanniadmin',  'devs'),
    ('bob',         'gaming'),
    ('søren',       'music'),
    ('mikkel',      'books'),
    ('alice',       'design'),
    ('sten',        'hardware'),
    ('hans',        'startups'),
    ('weiadmin',    'ai'),
    ('mikkeladmin', 'movies');

INSERT INTO user_administrates_community (username, community_name) 
VALUES
    ('kaiadmin',    'climbing'),
    ('hanniadmin',  'devs'),
    ('bob',         'gaming'),
    ('søren',       'music'),
    ('mikkel',      'books'),
    ('alice',       'design'),
    ('sten',        'hardware'),
    ('hans',        'startups'),
    ('weiadmin',    'ai'),
    ('mikkeladmin', 'movies');

INSERT INTO user_moderates_community (username, community_name) 
VALUES
    ('weiadmin',    'climbing'),
    ('mikkel',      'devs'),
    ('bob',         'gaming'),
    ('søren',       'music'),
    ('alice',       'books'),
    ('hanniadmin',  'design'),
    ('sten',        'hardware'),
    ('kaiadmin',    'startups'),
    ('mikkeladmin', 'ai'),
    ('hans',        'movies');

-- USER FOLLOWERS
INSERT INTO user_followers (target_username, follower_username)
VALUES
    ('kaiadmin',    'hanniadmin'),
    ('kaiadmin',    'weiadmin'),
    ('hanniadmin',  'kaiadmin'),
    ('hanniadmin',  'alice'),
    ('weiadmin',    'mikkel'),
    ('weiadmin',    'søren'),
    ('alice',       'hanniadmin'),
    ('alice',       'bob'),
    ('mikkel',      'hans'),
    ('mikkel',      'søren'),
    ('søren',       'bob'),
    ('søren',       'mikkel'),
    ('bob',         'sten'),
    ('bob',         'mikkel'),
    ('hans',        'alice'),
    ('hans',        'weiadmin'),
    ('sten',        'kaiadmin'),
    ('sten',        'bob'),
    ('mikkeladmin', 'hanniadmin'),
    ('mikkeladmin', 'alice'),
    ('kaiadmin',    'mikkel'),
    ('kaiadmin',    'hans'),
    ('hanniadmin',  'søren'),
    ('weiadmin',    'kaiadmin'),
    ('mikkel',      'hanniadmin'),
    ('alice',       'mikkeladmin'),
    ('søren',       'hans'),
    ('bob',         'kaiadmin'),
    ('hans',        'mikkeladmin'),
    ('sten',        'weiadmin');

-- Reset SERIAL sequences
SELECT setval(pg_get_serial_sequence('post','id'),     (SELECT COALESCE(MAX(id),1) FROM post),     true);
SELECT setval(pg_get_serial_sequence('comments','id'), (SELECT COALESCE(MAX(id),1) FROM comments), true);

COMMIT;