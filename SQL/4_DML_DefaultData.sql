use [dbWheelOfDeath];
go

insert into tblAdminType (AdminType) values
  ('SuperAdmin'),
  ('StandardAdmin');
go

insert into tblGlobalSettings default values;
go

insert into tblResult (IsWin, ResultType) values
    (1,'Win'),
    (0,'Lose'),
    (0,'Timeout');


-- Default super admin
insert into tblAccount (FirstName, LastName, Password, IsActiveFlag) values (
	'admin',
	'admin',
	'admin',
	1
);
declare @saId int = SCOPE_IDENTITY();
insert into tblAdmin (Id, FkAdminTypeId, Username) values (
	@saId,
	2,
	'admin'
);


use master;
go




