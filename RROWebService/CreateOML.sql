create table OMLScoreBoard (
[Node] int NOT NULL identity(1, 1) primary key,
[TeamId] nvarchar(50) not null,
[JudgeId] nvarchar(50) not null,
[Round] int not null default 1,
[StaysCorrectly] int not null default 0,
[LiesCorrectly] int not null default 0,
[PartiallyCorrect] int not null default 0,
[StaysIncorrectly] int not null default 0,
[LiesIncorrectly] int not null default 0,
[None] int not null default 0,
[BlueBlockState] int not null default 0,
[BlackBlockState] int not null default 0,
[FinishCorrectly] int not null default 0,
[BrokenWall] int not null default 0,
[TimeMils] int not null default 0,
[Saved] int not null default 0,

foreign key (JudgeId) references Judges(JudgeId),
foreign key (TeamId) references Teams(TeamId)
);