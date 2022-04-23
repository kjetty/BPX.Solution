----------------------------------------------------------------------------------------
-- ensure -T272 is added in the Startup parameters to avoid the identity to jump to 1001
----------------------------------------------------------------------------------------

--drop triggers
drop trigger TRG_Users_I;
drop trigger TRG_PLUMaster_UD;

--delete foreign key constraints
alter table RolePermits drop constraint FK_RolePermits_RoleId;
alter table RolePermits drop constraint FK_RolePermits_PermitId;
alter table UserRoles drop constraint FK_UserRoles_UserId;
alter table UserRoles drop constraint FK_UserRoles_RoleId;
alter table MenuPermits drop constraint FK_MenuPermits_MenuId;
alter table MenuPermits drop constraint FK_MenuPermits_PermitId;
alter table Users drop constraint FK_Users_PortalUUId;
alter table Users drop constraint FK_Users_LoginUUId;

--delete constraints
alter table Permits drop constraint UC_Permits_PermitEnum;
alter table Roles drop constraint UC_Roles_RoleName;
alter table Users drop constraint UC_Users_Email;
alter table Users drop constraint UC_Users_UserUUId
alter table Users drop constraint UC_Users_PortalUUId;
alter table Users drop constraint UC_Users_LoginUUId;
alter table Logins drop constraint UC_Logins_LoginName_PIVId;
alter table Logins drop constraint UC_Logins_RToken;
alter table Portals drop constraint UC_Portals_PToken;

GO

--drop tables
DROP TABLE RolePermits;
DROP TABLE UserRoles;
DROP TABLE MenuPermits;
DROP TABLE Permits;
DROP TABLE Roles;
DROP TABLE Users;
DROP TABLE Logins;
DROP TABLE Portals;
DROP TABLE PLUMaster;
DROP TABLE Menus;
DROP TABLE CacheKeys;

GO

--create tables
CREATE TABLE CacheKeys (
    CacheKeyName       	varchar (48)        NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (CacheKeyName ASC)
);

CREATE TABLE Menus (
    MenuId      		int IDENTITY(1,1)   NOT NULL,
    MenuName         	varchar (32)        NOT NULL,
    MenuDescription  	varchar (128)       NULL,
    MenuURL          	varchar (1024)      NULL,
    ParentMenuId    	INT		            NOT NULL,
    HLevel			    INT                 NOT NULL,
    OrderNumber  		INT                 NULL,
    TreePath            varchar(32)         NULL,
    StatusFlag          char(1)             NOT NULL,
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (MenuId ASC)
);

CREATE TABLE Permits (
    PermitId			int IDENTITY(1,1)   NOT NULL,
    PermitArea		    varchar (32)        NOT NULL,
    PermitController   	varchar (32)        NOT NULL,
    PermitName  	    varchar (32)        NOT NULL,
    PermitEnum		    varchar (64)        NOT NULL,
    StatusFlag          char(1)             NOT NULL,
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (PermitId ASC),
	CONSTRAINT UC_Permits_PermitEnum UNIQUE (PermitEnum)
);

CREATE TABLE Roles (
    RoleId			    int IDENTITY(1,1)   NOT NULL,
    RoleName			varchar (32)        NOT NULL,
    RoleDescription     varchar (128)       NULL,
    StatusFlag          char(1)             NOT NULL,
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (RoleId ASC),
	CONSTRAINT UC_Roles_RoleName UNIQUE (RoleName)
);

CREATE TABLE Portals (
    PortalUUId              varchar(24)         NOT NULL,
    PToken                  varchar(40)         NOT NULL,
    LastAccessTime          datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (PortalUUId ASC),
    CONSTRAINT UC_Portals_PToken UNIQUE (PToken)
);

CREATE INDEX IDX_Portals_PToken ON Portals (PToken);

CREATE TABLE Logins (
    LoginUUId               varchar(24)         NOT NULL,
    LoginName               varchar(32)         NULL,
    PasswordHash            varchar(128)        NULL,
    PIVId                   varchar(16)         NULL,
    LoginType               char(1)             NOT NULL,
    RToken                  varchar(40)         NOT NULL,
    LastLoginDate           datetime            NOT NULL,
    TransientUUId           varchar(24)         NULL,
    StatusFlag              char(1)             NOT NULL,
    ModifiedBy              int                 NOT NULL,
    ModifiedDate            datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (LoginUUId ASC),
    CONSTRAINT UC_Logins_LoginName_PIVId UNIQUE (LoginName,PIVId),
    CONSTRAINT UC_Logins_RToken UNIQUE (RToken)
);

CREATE INDEX IDX_Logins_LoginName ON Logins (LoginName);
CREATE INDEX IDX_Logins_PIVId ON Logins (PIVId);
CREATE INDEX IDX_Logins_RToken ON Logins (RToken);

CREATE TABLE Users (
    UserId              int IDENTITY(1,1)   NOT NULL,
    FirstName           varchar(32)         NOT NULL,
    LastName            varchar(32)         NOT NULL,
    Email               varchar(64)         NOT NULL,
    Mobile              varchar(16)         NULL,
    UserUUId			varchar(24)			NOT NULL,
	LoginUUId           varchar(24)         NOT NULL,
    PortalUUId          varchar(24)         NOT NULL,
    StatusFlag          char(1)             NOT NULL,       -- A (active), I (inactive), D (deleted), L (locked), R (archived)
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (UserId ASC),
	CONSTRAINT UC_Users_Email UNIQUE (Email),
    CONSTRAINT UC_Users_UserUUId UNIQUE (UserUUId),
    CONSTRAINT UC_Users_PortalUUId UNIQUE (PortalUUId),
    CONSTRAINT UC_Users_LoginUUId UNIQUE (LoginUUId),
    CONSTRAINT FK_Users_PortalUUId FOREIGN KEY (PortalUUId) REFERENCES Portals (PortalUUId),
    CONSTRAINT FK_Users_LoginUUId FOREIGN KEY (LoginUUId) REFERENCES Logins (LoginUUId)
);

CREATE INDEX IDX_Users_UserUUId ON Users (UserUUId);
CREATE INDEX IDX_Users_PortalUUId ON Users (PortalUUId);
CREATE INDEX IDX_Users_LoginUUId ON Users (LoginUUId);

CREATE TABLE PLUMaster (
    UserId				int					NOT NULL,
    UserUUId			varchar(24)			NOT NULL,
	LoginUUId           varchar(24)         NOT NULL,
    PortalUUId          varchar(24)         NOT NULL,
    CreatedDate         datetime            NOT NULL
);

CREATE TABLE RolePermits (
    RolePermitId	    int IDENTITY(1,1)   NOT NULL,
    RoleId		        int                 NOT NULL,
    PermitId	        int                 NOT NULL,
    StatusFlag          char(1)             NOT NULL,
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (RolePermitId ASC),
    CONSTRAINT FK_RolePermits_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (RoleId),
    CONSTRAINT FK_RolePermits_PermitId FOREIGN KEY (PermitId) REFERENCES Permits (PermitId)
);

CREATE TABLE UserRoles (
    UserRoleId        int IDENTITY(1,1)   NOT NULL,
    UserId            int                 NOT NULL,
    RoleId            int                 NOT NULL,
    StatusFlag        char(1)             NOT NULL,
    ModifiedBy        int                 NOT NULL,
    ModifiedDate      datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (UserRoleId ASC),
    CONSTRAINT FK_UserRoles_UserId FOREIGN KEY (UserId) REFERENCES Users (UserId),
    CONSTRAINT FK_UserRoles_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (RoleId)
);

CREATE TABLE MenuPermits (
    MenuPermitId      int IDENTITY(1,1)   NOT NULL,
    MenuId            int                 NOT NULL,
    PermitId          int                 NOT NULL,
    StatusFlag        char(1)             NOT NULL,
    ModifiedBy        int                 NOT NULL,
    ModifiedDate      datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (MenuPermitId ASC),
    CONSTRAINT FK_MenuPermits_MenuId FOREIGN KEY (MenuId) REFERENCES Menus (MenuId),
    CONSTRAINT FK_MenuPermits_PermitId FOREIGN KEY (PermitId) REFERENCES Permits (PermitId)
);

GO

--create triggers
CREATE TRIGGER TRG_Users_I
ON Users
AFTER INSERT
AS 
BEGIN   
    SET NOCOUNT ON;
    INSERT INTO PLUMaster (UserId, UserUUId, LoginUUId, PortalUUID, CreatedDate)
	SELECT ins.UserId, ins.UserUUId, ins.LoginUUId, ins.PortalUUId, ins.ModifiedDate FROM inserted ins
END;

GO

CREATE TRIGGER TRG_PLUMaster_UD ON PLUMaster
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    RAISERROR( 'PLUMaster is read only.', 16, 1 )
    ROLLBACK TRANSACTION
END

GO

--portals
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId001','PToken001',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId002','PToken002',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId003','PToken003',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId004','PToken004',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId005','PToken005',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId006','PToken006',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId007','PToken007',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId008','PToken008',getDate());
insert into Portals (PortalUUId,PToken,LastAccessTime) values ('PortalUUId009','PToken009',getDate());

--logins
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId001','one','AQAAAAEAACcQAAAAEJZUbWun++BJlFSiC352oPAgJ9UKJRXkKX4lD3hbjIsMSx6X+4qVUxkCIqTibouCkg==',null,'U','RToken001',getDate(),'A',1,getDate()); 
-- password is: test1111
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId002','two','AQAAAAEAACcQAAAAEB9JDwdihl7mM0l+TriHpUJ1RMwMBCPEcZyzx0I3jr76qDSCk4BrMXTw04I41QECsA==',null,'U','RToken002',getDate(),'A',1,getDate()); 
-- password is: test2222
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId003','three','AQAAAAEAACcQAAAAEEWfT9/y2LjqdjMDTjtpL6/atZbp0W35CU16ho4ZPc941tdMwWkQkjKmTG2CLpVNWw==',null,'U','RToken003',getDate(),'A',1,getDate()); 
-- password is: test3333
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId004','four','AQAAAAEAACcQAAAAEORU4xK4/14Hif4n9GskVGf0sARqO1Imofs0/hF8petfwQvJIatWXHFttl7fu9JMQw==',null,'U','RToken004',getDate(),'A',1,getDate()); 
-- password is: test4444
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId005','five','AQAAAAEAACcQAAAAEIqxBmeAMdu7VCHVUHdhWwE2gVFMMwAnj4gluKD3jH/7km5lCH24t0UmvVIV6Xldvg==',null,'U','RToken005',getDate(),'A',1,getDate()); 
-- password is: test5555
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId006','six','AQAAAAEAACcQAAAAEE0lGYLrUerJtuTesTovq5x0dYjJbXuHNoWxqc8fplpUatp+ZVGDK5tBO3fdv/KsBQ==',null,'U','RToken006',getDate(),'A',1,getDate()); 
-- password is: test6666
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId007','seven','AQAAAAEAACcQAAAAEGsbNHvJSb009T7pSeRxBrYvOKFrdNhO6eWrMjp6fxw/+eS9Iq7iFKhXW1LubkefEg==',null,'U','RToken007',getDate(),'A',1,getDate()); 
-- password is: test7777
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId008','eight','AQAAAAEAACcQAAAAEEfP/rkD26DdWFjUnEQ/IWyi/6kN8RAZGNcP0n6D+IHgfLX0y37Tm/FrxRX5RaiphQ==',null,'U','RToken008',getDate(),'A',1,getDate()); 
-- password is: test8888
insert into Logins (LoginUUId,LoginName,PasswordHash,PIVId,LoginType,Rtoken,LastLoginDate,StatusFlag,ModifiedBy,ModifiedDate) values ('LoginUUId009','nine','AQAAAAEAACcQAAAAEO6wAj4iqxtW6XZW7iCdKmKw3eWaT9nlla1BCdzFd4WfzPeJFyxJga8gBGPrFKXiRg==',null,'U','RToken009',getDate(),'A',1,getDate()); 
-- password is: test9999

--users
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('System','sysln','system.email@bpx.com','123-123-1234','UserUUId001','LoginUUId001','PortalUUId001','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('Process','processln','process.email@bpx.com','123-123-1234','UserUUId002','LoginUUId002','PortalUUId002','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','devln','developer.email@bpx.com','123-123-1234','UserUUId003','LoginUUId003','PortalUUId003','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('Superuser','superuserln','superuser.email@bpx.com','123-123-1234','UserUUId004','LoginUUId004','PortalUUId004','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('Admin','admln','admin.email@bpx.com','123-123-1234','UserUUId005','LoginUUId005','PortalUUId005','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('Manager','mgrln','manager.email@bpx.com','123-123-1234','UserUUId006','LoginUUId006','PortalUUId006','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('QualityAnalyst','qaln','qualityanalyst.email@bpx.com','123-123-1234','UserUUId007','LoginUUId007','PortalUUId007','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('fn08','08LN','entity08.email@bpx.com','123-123-1234','UserUUId008','LoginUUId008','PortalUUId008','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,UserUUId,LoginUUId,PortalUUId,StatusFlag,ModifiedBy,ModifiedDate) values ('fn09','09LN','entity09.email@bpx.com','123-123-1234','UserUUId009','LoginUUId009','PortalUUId009','A',1,getDate());

--roles
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('DeveloperRole','DeveloperRole','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('SuperuserRole','SuperuserRole','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('AdminRole','AdminRole','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('ManagerRole','ManagerRole','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('QARole','QARole','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('RegdUserRole','RegdUserRole','A',1,getDate());

--permits
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','ErrorLog','Index','Developer.ErrorLog.Index','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','ErrorLog','DownloadLog','Developer.ErrorLog.DownloadLog','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','PermitsGenerator','Index','Developer.Home.Index','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Create','Identity.Menu.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Read','Identity.Menu.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Update','Identity.Menu.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Delete','Identity.Menu.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','List','Identity.Menu.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Filter','Identity.Menu.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','ListDeleted','Identity.Menu.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Undelete','Identity.Menu.Undelete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','MenuPermits','Identity.Menu.MenuPermits','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','TreePath','Identity.Menu.TreePath','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Create','Identity.Permit.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Read','Identity.Permit.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Update','Identity.Permit.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Delete','Identity.Permit.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','List','Identity.Permit.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Filter','Identity.Permit.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','ListDeleted','Identity.Permit.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Undelete','Identity.Permit.Undelete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','PermitRolesMenus','Identity.Permit.PermitRolesMenus','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Create','Identity.Role.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Read','Identity.Role.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Update','Identity.Role.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Delete','Identity.Role.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','List','Identity.Role.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Filter','Identity.Role.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','ListDeleted','Identity.Role.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Undelete','Identity.Role.Undelete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','RolePermits','Identity.Role.RolePermits','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Create','Identity.User.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Read','Identity.User.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Update','Identity.User.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Delete','Identity.User.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','List','Identity.User.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Filter','Identity.User.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','ListDeleted','Identity.User.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Undelete','Identity.User.Undelete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','ChangePassword','Identity.User.ChangePassword','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','UserRoles','Identity.User.UserRoles','A',1,getDate());

--rolePermits
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','1','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','2','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','3','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','4','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','5','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','6','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','7','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','8','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','9','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','10','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','11','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','12','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','13','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','14','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','15','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','16','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','17','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','18','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','19','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','20','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','21','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','22','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','23','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','24','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','25','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','26','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','27','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','28','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','29','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','30','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','31','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','32','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','33','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','34','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','35','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','36','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','37','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','38','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','39','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','40','A',1,getDate());
insert into RolePermits (RoleId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('1','41','A',1,getDate());

--userRoles
insert into UserRoles (UserId,RoleId,StatusFlag,ModifiedBy,ModifiedDate) values ('3','1','A',1,getDate());

--menus
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Home',null,'/',0,1,1,'.1.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer',null,'/Developer',1,2,1,'.1.2.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity',null,'/Identity',1,2,2,'.1.3.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Errors Log',null,'/Developer/ErrorLog',2,3,1,'.1.2.4.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Permits Generator',null,'/Developer/PermitsGenerator',2,3,2,'.1.2.5.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Menu',null,'/Identity/Menu',3,3,1,'.1.3.6.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Permit',null,'/Identity/Permit',3,3,2,'.1.3.7.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Role',null,'/Identity/Role',3,3,3,'.1.3.8.','A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('User',null,'/Identity/User',3,3,4,'.1.3.9.','A',1,getDate());

--MenuPermits
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (4,1,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (4,2,'A',1,getDate());

insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (5,3,'A',1,getDate());

insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,4,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,5,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,6,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,7,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,8,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,9,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,10,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,11,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,12,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (6,13,'A',1,getDate());

insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,14,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,15,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,16,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,17,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,18,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,19,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,20,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,21,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (7,22,'A',1,getDate());

insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,23,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,24,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,25,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,26,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,27,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,28,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,29,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,30,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (8,31,'A',1,getDate());

insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,32,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,33,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,34,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,35,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,36,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,37,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,38,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,39,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,40,'A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values (9,41,'A',1,getDate());

----cte (common time execution) recursive hierarchy query
--WITH cte_menus AS (
--	SELECT MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, 1 AS hLevel, OrderNumber, CAST(MenuId AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate  
--	FROM Menus WHERE StatusFlag = 'A' AND ParentMenuId = 0
--	UNION ALL
--	SELECT m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, cte.hLevel + 1, m.OrderNumber, CAST(cte.TreePath + '.' + CAST(m.MenuId AS VARCHAR(32)) AS VARCHAR(32)) AS TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate 
--	FROM Menus m INNER JOIN cte_menus cte ON cte.MenuId = m.ParentMenuId WHERE m.StatusFlag = 'A'
--)
--SELECT MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, CAST('.' + TreePath + '.' AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate FROM cte_menus 
--ORDER BY MenuURL]

-- end of script
