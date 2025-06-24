CREATE TABLE [dbo].[tblGlobalSettings] (
    [Id]             SMALLINT IDENTITY (1, 1) NOT NULL,
    [HofRecordCount] BIGINT   DEFAULT ((10)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK ([Id]<=(1))
);

