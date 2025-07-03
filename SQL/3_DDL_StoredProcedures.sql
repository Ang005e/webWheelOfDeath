
use [dbWheelOfDeath];
go



drop procedure if exists [uspGamesByDifficulty];
go



create procedure [uspGamesByDifficulty] 
as

begin
select
    G.[Id], G.[Game], G.[FkDifficultyId], D.[Difficulty], D.[Id]
from
    tblGame G inner join
    tblDifficulty D on D.[Id] = G.[FkDifficultyId]
where
    (1=1)
order by
    D.[Id],
    G.[Game]
end;
go


create procedure [uspGamePopularity]
	@pStartDate datetime,
	@pEndDate datetime
as
begin
    select 
        G.[Game], G.[Id],
        D.[Difficulty],
        count(R.Id) as TotalGamesPlayed
    from [tblGameRecord] R inner join
		[tblGame] G on G.Id = R.FkGameId inner join
		[tblDifficulty] D on D.Id = G.FkDifficultyId
	where
		R.[Date] >= @pStartDate and
		R.[Date] <= @pEndDate
	group by
		G.[Game], G.[Id],
		D.[Difficulty]
end
go





use master;
go
