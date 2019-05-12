create table Categories
(
[CategoryId] nvarchar(50) not null primary key,
[FormPath] nvarchar(150) not null
);

CREATE TABLE Teams
(
[Id] nvarchar(50) not null primary key,
[Tour] nvarchar(100) not null,
[Polygon] int not null,
[CategoryId] nvarchar(50) not null,
foreign key (CategoryId) references Categories(CategoryId)
);

create table Judges
(
[Id] nvarchar(50) not null primary key,
[Name] nvarchar(150) not null,
[Tour] nvarchar(50) not null,
[Polygon] int not null
);