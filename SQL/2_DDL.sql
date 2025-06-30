use [dbWheelOfDeath];
go



drop view if exists vwHallOfFameTop;
go

create view vwHallOfFameTop as

select top(select top(1) HofRecordCount from [tblGlobalSettings])
	R.[Date], R.[ElapsedTime], R.[BalloonsPopped], R.[Misses],
	A.[FirstName], A.[LastName],
	G.[Game],
	D.[Difficulty]
from
	[tblGameRecord] R inner join
	[tblAccount] A on R.FkPlayerId = A.Id inner join
	[tblResult] RS on Rs.Id = R.FkResultId inner join
	[tblGame] G on R.FkGameId = G.Id inner join 
	[tblDifficulty] D on G.FkDifficultyId = D.Id
where
	RS.IsWin = 1
order by
	R.[ElapsedTime] asc

go



drop view if exists vwHallOfFameBottom;
go

create view vwHallOfFameBottom as

select top(select top(1) HofRecordCount from [tblGlobalSettings])
	R.[Date], R.[ElapsedTime], R.[BalloonsPopped], R.[Misses],
	A.[FirstName], A.[LastName],
	G.[Game],
	D.[Difficulty]
from
	[tblGameRecord] R inner join
	[tblAccount] A on R.FkPlayerId = A.Id inner join
	[tblResult] RS on Rs.Id = R.FkResultId inner join
	[tblGame] G on R.FkGameId = G.Id inner join 
	[tblDifficulty] D on G.FkDifficultyId = D.Id
where
	RS.IsWin = 1
order by
	R.[ElapsedTime] desc

go


use master;
go



--drop procedure if exists [uspCreatePerson];
--go
--create procedure [uspCreatePerson]
--	@pUsername	nvarchar(40),
--	@pFirstName	nvarchar(40),
--	@pLastName	nvarchar(40),
--	@pId		bigint output
--as
--begin
--	set nocount on; -- suppress reporting of rows affected

--	insert into []
--	(
--	)
--	VALUES
--	(
--	);

--	-- Immediately following the INSERT statement, we can
--	-- retrieve the unique Id that the system generated...
--	set @pId = cast(scope_identity() as bigint);

--end
--go
