CREATE TABLE [dbo].[SLUMaster] (
    [UserId]      INT          NOT NULL,
    [UserUUId]    VARCHAR (24) NOT NULL,
    [LoginUUId]   VARCHAR (24) NOT NULL,
    [SessonUUId]  VARCHAR (24) NOT NULL,
    [CreatedDate] DATETIME     NOT NULL
);


GO

CREATE TRIGGER TRG_SLUMaster_UD ON SLUMaster
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    RAISERROR( 'SLUMaster is read only.', 16, 1 )
    ROLLBACK TRANSACTION
END