CREATE TABLE [dbo].[Logins] (
    [LoginUUId]     VARCHAR (24)  NOT NULL,
    [CACCN]         VARCHAR (128) NULL,
    [CACId]         VARCHAR (16)  NULL,
    [CACSmall]      VARCHAR (16)  NULL,
    [CACLarge]      VARCHAR (16)  NULL,
    [LoginName]     VARCHAR (32)  NULL,
    [PasswordHash]  VARCHAR (128) NULL,
    [ADUserName]    VARCHAR (32)  NULL,
    [LastLoginDate] DATETIME      NOT NULL,
    [LoginType]     CHAR (1)      NOT NULL,
    [LToken]        VARCHAR (40)  NOT NULL,
    [StatusFlag]    CHAR (1)      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([LoginUUId] ASC),
    CONSTRAINT [UC_Logins_CACId_LoginName] UNIQUE NONCLUSTERED ([CACId] ASC, [LoginName] ASC),
    CONSTRAINT [UC_Logins_LToken] UNIQUE NONCLUSTERED ([LToken] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_Logins_LToken]
    ON [dbo].[Logins]([LToken] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Logins_CACId]
    ON [dbo].[Logins]([CACId] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Logins_LoginName]
    ON [dbo].[Logins]([LoginName] ASC);

