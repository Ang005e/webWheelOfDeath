use [master];
go

drop database if exists [dbWheelOfDeath];
go

create database [dbWheelOfDeath];
go

use [dbWheelOfDeath];
go


--			 [DB 04]
create table [tblAccount]
(
	[Id]				bigint			not null	primary key identity,
--	[DB 01]
	[FirstName]			varchar(100)	not null,
--	[DB 01]
	[LastName]			varchar(100)	not null,
--	[DB 01]				[DB 02]
	[Password]			varchar(255)	not null,	
--	[DB 05]
	[IsActiveFlag]		bit				not null	
);


------------------------ Accounts ------------------------

create table [tblAdminType]
(
	[Id]				bigint			not null	primary key identity,
	[AdminType]			varchar(255)	not null 
);

create table [tblAdmin]
(
	[Id]				bigint			not null	primary key foreign key references [tblAccount]([Id]), -- 3.5NF
	[FkAdminTypeId]		bigint			not null	foreign key references [tblAdminType]([Id]),
--													[DB 03]
	[Username]			varchar(200)	not null	constraint UQ_Admin_Username unique,	
);

create table [tblPlayer]
(															
	[Id]				bigint			not null	primary key foreign key references [tblAccount]([Id]), -- 3.5NF
--													[DB 03]
	[Username]			varchar(200)	not null	constraint UQ_Player_Username unique
);


-------------- Static global settings table --------------

create table [tblGlobalSettings]
(
	[Id]				smallint		not null	primary key identity	check (Id <= 1),
	[HofRecordCount]	bigint			not null	default(10) -- <<<<<<<< DEFAULT HALL OF FAME RECORD COUNT
);


---------------------- Games/Records ---------------------

create table [tblDifficulty]
(
	[Id]				bigint			not null	primary key identity,
--													Difficulty tag must be unique
	[Difficulty]		varchar(255)	not null	unique
);

create table [tblGame]
(
    [Id]				bigint			not null	primary key identity,
    [FkDifficultyId]	bigint			not null	foreign key references [tblDifficulty]([Id]),
    [Game]				varchar(255)	not null	constraint UQ_Game unique,
    [Attempts]			smallint		not null,
    [Misses]			smallint		not null,
    [Duration]			bigint			not null,
    [MinBalloons]		smallint		not null,	
    [MaxBalloons]		smallint		not null,	
	[IsActiveFlag]		bit				not null	default(1),	-- <<<<<<<< Games are active on creation

--	Duration must be less than 60 minutes.
	constraint chk_duration_under_3600000 check (Duration <= 3600000),
--	MinBalloons must be less than MaxBalloons.
	constraint chk_minballoons_not_exceeds_maxballoons check (MinBalloons <= MaxBalloons),
--	MaxBalloons must not exceed attempts.
	constraint chk_attempts_not_exceeds_maxballoons check (MaxBalloons <= Attempts),
--	Attempt count cannot exceed miss count.
	constraint chk_attempts_exceeds_misses check (Attempts > Misses)
);

create table [tblResult]
(
	[Id]				bigint			not null	primary key, -- no identity, manual insert, because Mr Majidi made his EnumGameStatus all stupid and incorrect and OH MY GOD THE TROUBLE I WENT TO YOU BLOODY IT'S 6:28AM IN TH EMORNING I SHOULD BE ASLEEP
	[IsWin]				bit				not null,
	[ResultType]		varchar(255)
);

create table [tblGameRecord](
    [Id]				bigint			not null	primary key identity,
    [FkGameId]			bigint			not null	foreign key references [tblGame](Id),
    [FkPlayerId]		bigint			not null	foreign key references [tblPlayer](Id),
    [FkResultId]		bigint			not null	foreign key references [tblResult](Id),
    [Date]				datetime		not null,	
	[ElapsedTime]		bigint			not null,	
    [BalloonsPopped]	smallint		not null,
    [Misses]			smallint		not null
);


use master;
go