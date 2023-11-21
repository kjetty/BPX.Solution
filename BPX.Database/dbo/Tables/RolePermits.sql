CREATE TABLE [dbo].[RolePermits] (
    [RolePermitId] INT      IDENTITY (1, 1) NOT NULL,
    [RoleId]       INT      NOT NULL,
    [PermitId]     INT      NOT NULL,
    [StatusFlag]   CHAR (1) NOT NULL,
    [ModifiedBy]   INT      NOT NULL,
    [ModifiedDate] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([RolePermitId] ASC),
    CONSTRAINT [FK_RolePermits_PermitId] FOREIGN KEY ([PermitId]) REFERENCES [dbo].[Permits] ([PermitId]),
    CONSTRAINT [FK_RolePermits_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
);

