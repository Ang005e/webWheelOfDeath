USE [dbWheelOfDeath];
GO


INSERT INTO tblDifficulty (Difficulty) VALUES ('Easy'), ('Medium'), ('Hard');

/*==============================================================
  3. Accounts (10 total)
     – Ids will be 1-10 in insertion order
     – Passwords meet >=12-char / mixed-set rule
==============================================================*/
INSERT INTO tblAccount (FirstName, LastName, Password, IsActiveFlag) VALUES
    ('John',   'Super',    'AAaa11!!BBbb', 1), -- 1
    ('Sarah',  'Std',      'BBbb22@@CCcc', 1), -- 2
    ('Peter',  'Player',   'CCcc33##DDdd', 1), -- 3
    ('Lucy',   'Gamer',    'DDdd44$$EEee', 1), -- 4
    ('Alex',   'Nguyen',   'EEee55%%FFff', 1), -- 5
    ('Maria',  'Smith',    'FFff66^^GGgg', 1), -- 6
    ('Carlos', 'Diaz',     'GGgg77&&HHhh', 1), -- 7
    ('Mei',    'Li',       'HHhh88**IIii', 1), -- 8
    ('Sam',    'Walker',   'IIii99((JJjj', 1), -- 9
    ('Nora',   'Brown',    'JJjj00))KKkk', 1); -- 10

/*==============================================================
  4. Roles
==============================================================*/
-- Admins (Ids 1 & 2)
INSERT INTO tblAdmin (Id, FkAdminTypeId, Username) VALUES
    (2, 2, 'john_super'),
    (3, 3, 'sarah_admin');

-- Players (Ids 3-10)
INSERT INTO tblPlayer (Id, Username) VALUES
    (3,  'player_peter'),
    (4,  'gamer_lucy'),
    (5,  'alex_ng'),
    (6,  'maria_s'),
    (7,  'carlos_d'),
    (8,  'mei_li'),
    (9,  'sam_w'),
    (10, 'nora_b');

/*==============================================================
  5. Games (7 rows, durations ≥ 90 000 ms)
==============================================================*/
INSERT INTO tblGame
    (FkDifficultyId, Game,                Attempts, Misses, Duration, MinBalloons, MaxBalloons)
VALUES
    (1, 'Balloon Pop',       10, 2, 120000, 1, 5),   -- Id 1
    (2, 'Balloon Blitz',     15, 3, 200000, 2, 10),  -- Id 2
    (1, 'Balloon Rush',       8, 4,  90000, 1, 4),   -- Id 3
    (1, 'Balloon Warm-Up',   12, 2, 180000, 1, 6),   -- Id 4
    (2, 'Balloon Marathon',  25, 5, 420000, 2, 15),  -- Id 5
    (2, 'Balloon Frenzy',    18, 4, 240000, 2, 12),  -- Id 6
    (3, 'Balloon Nightmare', 30, 8, 600000, 3, 20);  -- Id 7

/*==============================================================
  6. Game records (20 rows)
==============================================================*/
INSERT INTO tblGameRecord
    (FkGameId, FkPlayerId, FkResultId, [Date], ElapsedTime, BalloonsPopped, Misses)
VALUES
    (1, 3, 1, '2025-05-20T09:00:00', 2000,  8, 2),
    (2, 3, 2, '2025-05-20T09:30:00', 2000, 12, 3),
    (3, 3, 3, '2025-05-20T10:00:00', 2000,  4, 4),
    (1, 4, 2, '2025-05-20T10:30:00', 2000,  6, 4),
    (4, 5, 1, '2025-05-20T11:00:00', 2000, 10, 2),
    (6, 6, 2, '2025-05-20T11:05:00', 2000, 14, 4),
    (7, 7, 3, '2025-05-20T11:10:00', 2000, 17, 8),
    (2, 8, 1, '2025-05-20T11:15:00', 2000, 15, 3),
    (1, 9, 2, '2025-05-20T11:20:00', 2000,  7, 4),
    (4, 10,1, '2025-05-20T11:25:00', 2000, 11, 1),
    (6, 5, 2, '2025-05-20T11:30:00', 2000, 12, 6),
    (1, 6, 1, '2025-05-20T11:35:00', 2000,  9, 1),
    (2, 7, 2, '2025-05-20T11:40:00', 2000, 13, 4),
    (6, 8, 3, '2025-05-20T11:45:00', 2000, 10, 4),
    (5, 9, 1, '2025-05-20T11:50:00', 2000, 23, 3),
    (2, 10,2, '2025-05-20T11:55:00', 2000, 14, 5),
    (7, 3, 1, '2025-05-20T12:00:00', 2000, 25, 5),
    (5, 4, 3, '2025-05-20T12:05:00', 2000, 19, 5),
    (7, 3, 2, '2025-05-20T12:10:00', 2000, 18, 8),
    (5, 4, 1, '2025-05-20T12:15:00', 2000, 24, 2);
GO

use master;
go