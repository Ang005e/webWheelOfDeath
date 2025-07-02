use [dbWheelOfDeath];
go



drop view if exists vwPlayerWithAccount;
go
create view vwPlayerWithAccount as
select P.Username, AC.* from [tblPlayer] P inner join [tblAccount] AC on AC.[Id] = P.[Id]
go


drop view if exists vwAdminWithAccount;
go
create view vwAdminWithAccount as
select A.[FkAdminTypeId], A.Username, AC.* from [tblAdmin] A inner join [tblAccount] AC on A.[Id] = AC.[Id]
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

