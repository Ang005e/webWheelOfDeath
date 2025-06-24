CREATE TABLE [dbo].[tblAccount] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [FirstName]    VARCHAR (100) NOT NULL,
    [LastName]     VARCHAR (100) NOT NULL,
    [Password]     VARCHAR (255) NOT NULL,
    [IsActiveFlag] BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

