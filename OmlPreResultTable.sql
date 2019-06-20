DROP TABLE OMLPreResults;
CREATE TABLE OMLPreResults(
[Node] INT NOT NULL PRIMARY KEY,
[TeamId] NVARCHAR(50) NOT NULL,
[Tour] INT NOT NULL,
[Round] INT NOT NULL,
[JudgeId] NVARCHAR(50) NOT NULL,
[Polygon] INT NOT NULL,
[RedBlockState] INT NULL,
[YellowBlockState] INT NULL,
[GreenBlockState] INT NULL,
[WhiteBlock1State] INT NULL,
[WhiteBlock2State] INT NULL,
[BlueBlockState] INT NULL,
[BattaryBlock1State] INT NULL,
[BattaryBlock2State] INT NULL,
[RobotState] INT NULL,
[Wall1State] INT NULL,
[Wall2State] INT NULL,
[AdditionalTask] INT NULL,
[Time1] INT NULL,
[Time2] INT NULL,
[Saved] INT NOT NULL
)