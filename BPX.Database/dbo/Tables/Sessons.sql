CREATE TABLE [dbo].[Sessons] (
    [SessonUUId]     VARCHAR (24) NOT NULL,
    [SToken]         VARCHAR (40) NOT NULL,
    [LastAccessTime] DATETIME     NOT NULL,
    PRIMARY KEY CLUSTERED ([SessonUUId] ASC),
    CONSTRAINT [UC_Sessons_SToken] UNIQUE NONCLUSTERED ([SToken] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_Sessons_SToken]
    ON [dbo].[Sessons]([SToken] ASC);

