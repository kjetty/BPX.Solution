CREATE TABLE [dbo].[MenuPermits] (
    [MenuPermitId] INT      IDENTITY (1, 1) NOT NULL,
    [MenuId]       INT      NOT NULL,
    [PermitId]     INT      NOT NULL,
    [StatusFlag]   CHAR (1) NOT NULL,
    [ModifiedBy]   INT      NOT NULL,
    [ModifiedDate] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([MenuPermitId] ASC),
    CONSTRAINT [FK_MenuPermits_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([MenuId]),
    CONSTRAINT [FK_MenuPermits_PermitId] FOREIGN KEY ([PermitId]) REFERENCES [dbo].[Permits] ([PermitId])
);

