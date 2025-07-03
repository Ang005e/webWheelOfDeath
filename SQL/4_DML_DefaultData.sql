use [dbWheelOfDeath];
go

insert into tblAdminType (AdminType) values
  ('SuperAdmin'),
  ('StandardAdmin');
go

insert into tblGlobalSettings default values;
go

insert into tblResult (Id, IsWin, ResultType) values
    (2, 1,'Won'),
    (3, 0,'Killed'),
    (4, 0,'Timed_Out'),
	(5, 0,'Exceeded_Throws');
go

-- Default super admin
insert into tblAccount (FirstName, LastName, Password, IsActiveFlag) values (
	'admin',
	'admin',
	'adminAdmin11!',
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




