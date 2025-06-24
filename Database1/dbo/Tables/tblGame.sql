CREATE TABLE [dbo].[tblGame] (
    [Id]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [FkDifficultyId] BIGINT        NOT NULL,
    [Game]           VARCHAR (255) NOT NULL,
    [Attempts]       SMALLINT      NOT NULL,
    [Misses]         SMALLINT      NOT NULL,
    [Duration]       BIGINT        NOT NULL,
    [MinBalloons]    SMALLINT      NOT NULL,
    [MaxBalloons]    SMALLINT      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK ([MaxBalloons]<=[Attempts]),
    CHECK ([MinBalloons]<[MaxBalloons])
);

