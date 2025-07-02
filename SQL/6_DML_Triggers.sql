
use [dbWheelOfDeath];
go

/* ─────────────  vwPlayerWithAccount  ───────────── */

if object_id('trVwPlayerWithAccountInsert','TR') is not null drop trigger tr_vwPlayerWithAccount_ins;
go
create trigger trVwPlayerWithAccountInsert
on vwPlayerWithAccount
instead of insert
as
begin
    set nocount on;

    declare @t table (Id bigint, Username varchar(200));

    merge tblAccount as tgt
    using (select FirstName, LastName, Password, IsActiveFlag, Username from inserted) as src
    on 1=0
    when not matched then
        insert (FirstName, LastName, Password, IsActiveFlag)
        values (src.FirstName, src.LastName, src.Password, src.IsActiveFlag)
    output inserted.Id, src.Username into @t;

    insert into tblPlayer (Id, Username)
    select Id, Username
    from @t;
end;
go

if object_id('trVwPlayerWithAccountUpdate','TR') is not null drop trigger tr_vwPlayerWithAccount_upd;
go
create trigger trVwPlayerWithAccountUpdate
on vwPlayerWithAccount
instead of update
as
begin
    set nocount on;

    update A
    set A.FirstName      = i.FirstName,
        A.LastName       = i.LastName,
        A.Password       = i.Password,
        A.IsActiveFlag   = i.IsActiveFlag
    from tblAccount A
    join inserted i on A.Id = i.Id;

    update P
    set P.Username = i.Username
    from tblPlayer P
    join inserted i on P.Id = i.Id;
end;
go

if object_id('trVwPlayerWithAccountDelete','TR') is not null drop trigger tr_vwPlayerWithAccount_del;
go
create trigger trVwPlayerWithAccountDelete
on vwPlayerWithAccount
instead of delete
as
begin
    set nocount on;

    delete P
    from tblPlayer P
    join deleted d on P.Id = d.Id;

    delete A
    from tblAccount A
    join deleted d on A.Id = d.Id;
end;
go



/* ─────────────  vwAdminWithAccount  ───────────── */

if object_id('trVwAdminWithAccountInsert','TR') is not null drop trigger tr_vwAdminWithAccount_ins;
go
create trigger trVwAdminWithAccountInsert
on vwAdminWithAccount
instead of insert
as
begin
    set nocount on;

    declare @t table (Id bigint, Username varchar(200), FkAdminTypeId bigint);

    merge tblAccount as tgt
    using (select FirstName, LastName, Password, IsActiveFlag, Username, FkAdminTypeId from inserted) as src
    on 1=0
    when not matched then
        insert (FirstName, LastName, Password, IsActiveFlag)
        values (src.FirstName, src.LastName, src.Password, src.IsActiveFlag)
    output inserted.Id, src.Username, src.FkAdminTypeId into @t;

    insert into tblAdmin (Id, Username, FkAdminTypeId)
    select Id, Username, FkAdminTypeId
    from @t;
end;
go

if object_id('trVwAdminWithAccountUpdate','TR') is not null drop trigger tr_vwAdminWithAccount_upd;
go
create trigger trVwAdminWithAccountUpdate
on vwAdminWithAccount
instead of update
as
begin
    set nocount on;

    update A
    set A.FirstName      = i.FirstName,
        A.LastName       = i.LastName,
        A.Password       = i.Password,
        A.IsActiveFlag   = i.IsActiveFlag
    from tblAccount A
    join inserted i on A.Id = i.Id;

    update AD
    set AD.Username      = i.Username,
        AD.FkAdminTypeId = i.FkAdminTypeId
    from tblAdmin AD
    join inserted i on AD.Id = i.Id;
end;
go

if object_id('trVwAdminWithAccountDelete','TR') is not null drop trigger tr_vwAdminWithAccount_del;
go
create trigger trVwAdminWithAccountDelete
on vwAdminWithAccount
instead of delete
as
begin
    set nocount on;

    delete AD
    from tblAdmin AD
    join deleted d on AD.Id = d.Id;

    delete A
    from tblAccount A
    join deleted d on A.Id = d.Id;
end;
go


use master;
go