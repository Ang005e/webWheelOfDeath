CREATE TABLE [dbo].[tblGameRecord] (
    [Id]             BIGINT   NOT NULL,
    [FkGameId]       BIGINT   NOT NULL,
    [FkPlayerId]     BIGINT   NOT NULL,
    [FkResultId]     BIGINT   NOT NULL,
    [Date]           DATETIME NOT NULL,
    [BalloonsPopped] SMALLINT NOT NULL,
    [Misses]         SMALLINT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([FkGameId]) REFERENCES [dbo].[tblGame] ([Id]),
    FOREIGN KEY ([FkPlayerId]) REFERENCES [dbo].[tblPlayer] ([Id]),
    FOREIGN KEY ([FkResultId]) REFERENCES [dbo].[tblResult] ([Id])
);

