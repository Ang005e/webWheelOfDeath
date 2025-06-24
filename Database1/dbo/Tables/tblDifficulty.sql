CREATE TABLE [dbo].[tblDifficulty] (
    [Id]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [Difficulty] VARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Difficulty] ASC)
);

