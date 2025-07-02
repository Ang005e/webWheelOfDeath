use [dbWheelOfDeath];
go


insert into tblDifficulty (Difficulty) values ('Easy'), ('Medium'), ('Hard');


insert into tblAccount (FirstName, LastName, Password, IsActiveFlag) values
    ('John',   'Super',    'AAaa11!!BBbb', 1);
declare @pSuperAdminId bigint = SCOPE_IDENTITY()

insert into tblAccount (FirstName, LastName, Password, IsActiveFlag) values
    ('Sarah',  'Std',      'BBbb22@@CCcc', 1);
declare @pStandardAdminId bigint = SCOPE_IDENTITY()


insert into tblAccount (FirstName, LastName, Password, IsActiveFlag) values
    ('Peter',  'Player',   'CCcc33##DDdd', 1),
    ('Lucy',   'Gamer',    'DDdd44$$EEee', 1),
    ('Alex',   'Nguyen',   'EEee55%%FFff', 1),
    ('Maria',  'Smith',    'FFff66^^GGgg', 1),
    ('Carlos', 'Diaz',     'GGgg77&&HHhh', 1),
    ('Mei',    'Li',       'HHhh88**IIii', 1),
    ('Sam',    'Walker',   'IIii99((JJjj', 1),
    ('Nora',   'Brown',    'JJjj00))KKkk', 1);


-- Admins
insert into tblAdmin (Id, FkAdminTypeId, Username) values
    (@pSuperAdminId, 2, 'john_super'),
    (@pStandardAdminId, 1, 'sarah_admin');

-- Players
insert into tblPlayer (Id, Username) values
    (4,  'player_peter'),
    (5,  'gamer_lucy'),
    (6,  'alex_ng'),
    (7,  'maria_s'),
    (8,  'carlos_d'),
    (9,  'mei_li'),
    (10,  'sam_w'),
    (11, 'nora_b');


insert into tblGame
    (FkDifficultyId, Game, Attempts, Misses, Duration, MinBalloons, MaxBalloons, IsActiveFlag)
values
    (1, 'Balloon Pop',       10, 2, 120000, 1, 5,  1),
    (2, 'Balloon Blitz',     15, 3, 200000, 2, 10, 1),
    (1, 'Balloon Rush',       8, 4,  90000, 1, 4,  1),
    (1, 'Balloon Warm-Up',   12, 2, 180000, 1, 6,  1),
    (2, 'Balloon Marathon',  25, 5, 420000, 2, 15, 1),
    (2, 'Balloon Frenzy',    18, 4, 240000, 2, 12, 1),
    (3, 'Balloon Nightmare', 30, 8, 600000, 3, 20, 1);


-- Get the actual IDs from tblResult
declare @WinResultId bigint = (select Id from tblResult where ResultType = 'Won');
declare @KilledResultId bigint = (select Id from tblResult where ResultType = 'Killed');
declare @TimeoutResultId bigint = (select Id from tblResult where ResultType = 'Timed_Out');

insert into tblGameRecord
    (FkGameId, FkPlayerId, FkResultId, [Date], ElapsedTime, BalloonsPopped, Misses)
values
    (1, 5, @WinResultId,     '2025-05-20T09:00:00', 95000,  8, 2),
    (2, 11, @KilledResultId,    '2025-05-20T09:30:00', 180000, 12, 3),
    (3, 6, @TimeoutResultId, '2025-05-20T10:00:00', 90000,  4, 4),
    (1, 11, @KilledResultId,    '2025-05-20T10:30:00', 110000, 6, 4),
    (4, 5, @WinResultId,     '2025-05-20T11:00:00', 150000, 10, 2),
    (6, 6, @KilledResultId,    '2025-05-20T11:05:00', 220000, 14, 4),
    (7, 7, @TimeoutResultId, '2025-05-20T11:10:00', 600000, 17, 8),
    (2, 8, @WinResultId,     '2025-05-20T11:15:00', 175000, 15, 3),
    (1, 9, @KilledResultId,    '2025-05-20T11:20:00', 118000, 7, 4),
    (4, 10,@WinResultId,     '2025-05-20T11:25:00', 165000, 11, 1),
    (6, 5, @KilledResultId,    '2025-05-20T11:30:00', 235000, 12, 6),
    (1, 6, @WinResultId,     '2025-05-20T11:35:00', 98000,  9, 1),
    (2, 7, @KilledResultId,    '2025-05-20T11:40:00', 190000, 13, 4),
    (6, 8, @TimeoutResultId, '2025-05-20T11:45:00', 240000, 10, 4),
    (5, 9, @WinResultId,     '2025-05-20T11:50:00', 380000, 23, 3),
    (2, 10,@KilledResultId,    '2025-05-20T11:55:00', 195000, 14, 5),
    (7, 8, @WinResultId,     '2025-05-20T12:00:00', 520000, 25, 5),
    (5, 4, @TimeoutResultId, '2025-05-20T12:05:00', 420000, 19, 5),
    (7, 11, @KilledResultId,    '2025-05-20T12:10:00', 580000, 18, 8),
    (5, 4, @WinResultId,     '2025-05-20T12:15:00', 390000, 24, 2);
go

use master;
go