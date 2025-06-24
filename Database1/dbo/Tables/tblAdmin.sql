CREATE TABLE [dbo].[tblAdmin] (
    [Id]            BIGINT        NOT NULL,
    [FkAdminTypeId] BIGINT        NOT NULL,
    [Username]      VARCHAR (200) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([FkAdminTypeId]) REFERENCES [dbo].[tblAdminType] ([Id]),
    FOREIGN KEY ([Id]) REFERENCES [dbo].[tblAccount] ([Id]),
    CONSTRAINT [UQ_Admin_Username] UNIQUE NONCLUSTERED ([Username] ASC)
);

