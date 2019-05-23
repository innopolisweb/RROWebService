create table OMLScoreBoard (
[Node] int NOT NULL identity(1, 1) primary key,
[TeamId] nvarchar(50) not null,
[JudgeId] nvarchar(50) not null,
[Round] int not null default 1,
[Polygon] int not null,
[StaysCorrectly] int null,
[LiesCorrectly] int null,
[PartiallyCorrect] int null,
[StaysIncorrectly] int null,
[LiesIncorrectly] int null,
[None] int null,
[BlueBlockState] int null,
[BlackBlockState] int null,
[FinishCorrectly] int null,
[BrokenWall] int null,
[TimeMils] int null,
[Saved] int null,

foreign key (JudgeId) references Judges(JudgeId),
foreign key (TeamId) references Teams(TeamId)
);

