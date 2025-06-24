CREATE TABLE [dbo].[tblResult] (
    [Id]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [IsWin]      BIT           NOT NULL,
    [ResultType] VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

