alter table Judges add [Pass] nvarchar(50) null;
update Judges
set Pass = 1;
alter table Judges alter column Pass nvarchar(50) not null;