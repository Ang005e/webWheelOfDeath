use [dbWheelOfDeath];
go

insert into tblAdminType (AdminType) values
  ('SuperAdmin'),
  ('StandardAdmin');
go

insert into tblGlobalSettings default values;
go

insert into tblResult (IsWin, ResultType) values
    (1,'Won'),
    (0,'Killed'),
    (0,'Timed_Out'),
	(0,'Exceeded_Throws');
go


-- Default super admin
insert into tblAccount (FirstName, LastName, Password, IsActiveFlag) values (
	'admin',
	'admin',
	'admin',
	2
);

declare @saId int = SCOPE_IDENTITY();

insert into tblAdmin (Id, FkAdminTypeId, Username) values (
	@saId,
	2,
	'admin'
);


use master;
go




