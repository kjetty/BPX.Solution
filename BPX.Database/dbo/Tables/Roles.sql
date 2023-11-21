CREATE TABLE [dbo].[Roles] (
    [RoleId]          INT           IDENTITY (1, 1) NOT NULL,
    [RoleName]        VARCHAR (32)  NOT NULL,
    [RoleDescription] VARCHAR (128) NULL,
    [StatusFlag]      CHAR (1)      NOT NULL,
    [ModifiedBy]      INT           NOT NULL,
    [ModifiedDate]    DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([RoleId] ASC),
    CONSTRAINT [UC_Roles_RoleName] UNIQUE NONCLUSTERED ([RoleName] ASC)
);

