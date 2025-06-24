CREATE TABLE [dbo].[tblPlayer] (
    [Id]       BIGINT        NOT NULL,
    [Username] VARCHAR (200) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([Id]) REFERENCES [dbo].[tblAccount] ([Id]),
    UNIQUE NONCLUSTERED ([Username] ASC)
);

