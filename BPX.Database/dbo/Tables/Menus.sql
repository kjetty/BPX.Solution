CREATE TABLE [dbo].[Menus] (
    [MenuId]          INT            IDENTITY (1, 1) NOT NULL,
    [MenuName]        VARCHAR (32)   NOT NULL,
    [MenuDescription] VARCHAR (128)  NULL,
    [MenuURL]         VARCHAR (1024) NULL,
    [ParentMenuId]    INT            NOT NULL,
    [HLevel]          INT            NOT NULL,
    [OrderNumber]     INT            NULL,
    [TreePath]        VARCHAR (32)   NULL,
    [StatusFlag]      CHAR (1)       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([MenuId] ASC)
);

