CREATE TABLE [dbo].[Permits] (
    [PermitId]         INT          IDENTITY (1, 1) NOT NULL,
    [PermitArea]       VARCHAR (32) NOT NULL,
    [PermitController] VARCHAR (32) NOT NULL,
    [PermitName]       VARCHAR (32) NOT NULL,
    [PermitEnum]       VARCHAR (64) NOT NULL,
    [StatusFlag]       CHAR (1)     NOT NULL,
    [ModifiedBy]       INT          NOT NULL,
    [ModifiedDate]     DATETIME     NOT NULL,
    PRIMARY KEY CLUSTERED ([PermitId] ASC),
    CONSTRAINT [UC_Permits_PermitEnum] UNIQUE NONCLUSTERED ([PermitEnum] ASC)
);

