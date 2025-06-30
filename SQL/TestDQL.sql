use dbWheelOfDeath;
go

select 
	A.Id, [AC].FirstName, [AC].LastName, [AT].AdminType 
from 
	[tblAdmin] [A] inner join 
	[tblAccount] [AC] on [A].Id = [AC].Id inner join 
	[tblAdminType] [AT] on [A].FkAdminTypeId = [AT].Id
;
go

select 
	*
from 
	tblAccount
;
go


-- Get the config settings
select 
	*
from
	tblGlobalSettings
;

use master

-- Get data from database to populate constructor params.
-- Player inetrface berfore admin interface, log in, select and play game, get values from database.
-- Create admin interface.

-- Pages are in the view
-- Database and class libraries are within model
-- Controller is the API for network requests