
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