CREATE TABLE [dbo].[Errors] (
    [ErrorId]   INT            IDENTITY (1, 1) NOT NULL,
    [ErrorData] VARCHAR (8000) NULL,
    [ErrorDate] DATETIME       NOT NULL,
    PRIMARY KEY CLUSTERED ([ErrorId] ASC)
);

