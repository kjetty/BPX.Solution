CREATE TABLE [dbo].[Users] (
    [UserId]       INT          IDENTITY (1, 1) NOT NULL,
    [FirstName]    VARCHAR (32) NOT NULL,
    [LastName]     VARCHAR (32) NOT NULL,
    [Email]        VARCHAR (64) NOT NULL,
    [Mobile]       VARCHAR (16) NULL,
    [UserUUId]     VARCHAR (24) NOT NULL,
    [LoginUUId]    VARCHAR (24) NOT NULL,
    [SessonUUId]   VARCHAR (24) NOT NULL,
    [StatusFlag]   CHAR (1)     NOT NULL,
    [ModifiedBy]   INT          NOT NULL,
    [ModifiedDate] DATETIME     NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_Users_LoginUUId] FOREIGN KEY ([LoginUUId]) REFERENCES [dbo].[Logins] ([LoginUUId]),
    CONSTRAINT [FK_Users_SessonUUId] FOREIGN KEY ([SessonUUId]) REFERENCES [dbo].[Sessons] ([SessonUUId]),
    CONSTRAINT [UC_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC),
    CONSTRAINT [UC_Users_LoginUUId] UNIQUE NONCLUSTERED ([LoginUUId] ASC),
    CONSTRAINT [UC_Users_SessonUUId] UNIQUE NONCLUSTERED ([SessonUUId] ASC),
    CONSTRAINT [UC_Users_UserUUId] UNIQUE NONCLUSTERED ([UserUUId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_Users_LoginUUId]
    ON [dbo].[Users]([LoginUUId] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Users_SessonUUId]
    ON [dbo].[Users]([SessonUUId] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Users_UserUUId]
    ON [dbo].[Users]([UserUUId] ASC);


GO

CREATE TRIGGER TRG_Adt_Users
ON Users
FOR INSERT, UPDATE, DELETE
AS
  IF (SELECT COUNT(*) FROM inserted) > 0
      BEGIN
          IF (SELECT COUNT(*) FROM deleted) > 0
              BEGIN
                -- update!
                INSERT INTO DbBestPracticesAudit.DBO.Users (UserId, FirstName, LastName, Email, Mobile, UserUUId, LoginUUId, SessonUUId, StatusFlag, ModifiedBy, ModifiedDate, AuditType)
                SELECT ins.UserId, ins.FirstName, ins.LastName, ins.Email, ins.Mobile, ins.UserUUId, ins.LoginUUId, ins.SessonUUId, ins.StatusFlag, ins.ModifiedBy, ins.ModifiedDate, 'U' FROM inserted ins
              END
          ELSE
              BEGIN
                -- insert!
                INSERT INTO DbBestPracticesAudit.DBO.Users (UserId, FirstName, LastName, Email, Mobile, UserUUId, LoginUUId, SessonUUId, StatusFlag, ModifiedBy, ModifiedDate, AuditType)
                SELECT ins.UserId, ins.FirstName, ins.LastName, ins.Email, ins.Mobile, ins.UserUUId, ins.LoginUUId, ins.SessonUUId, ins.StatusFlag, ins.ModifiedBy, ins.ModifiedDate, 'I' FROM inserted ins
              END
      END
  ELSE
      BEGIN
        -- delete!
        INSERT INTO DbBestPracticesAudit.DBO.Users (UserId, FirstName, LastName, Email, Mobile, UserUUId, LoginUUId, SessonUUId, StatusFlag, ModifiedBy, ModifiedDate, AuditType)
        SELECT del.UserId, del.FirstName, del.LastName, del.Email, del.Mobile, del.UserUUId, del.LoginUUId, del.SessonUUId, del.StatusFlag, del.ModifiedBy, del.ModifiedDate, 'D' FROM deleted del
      END
GO

--create triggers
CREATE TRIGGER TRG_Users_I
ON Users
AFTER INSERT
AS 
BEGIN   
    SET NOCOUNT ON;
    INSERT INTO SLUMaster (UserId, UserUUId, LoginUUId, SessonUUID, CreatedDate)
	SELECT ins.UserId, ins.UserUUId, ins.LoginUUId, ins.SessonUUId, ins.ModifiedDate FROM inserted ins
END;