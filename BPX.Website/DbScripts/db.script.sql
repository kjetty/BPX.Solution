--delete foreign key constraints
alter table Logins drop constraint FK_Logins_Users_UserId;
alter table RolePermits drop constraint FK_Roles_Permits_RoleId;
alter table RolePermits drop constraint FK_Roles_Permits_PermitId;
alter table UserRoles drop constraint FK_Users_Roles_UserId;
alter table UserRoles drop constraint FK_Users_Roles_RoleId;
alter table MenuPermits drop constraint FK_Menus_Permits_MenuId;
alter table MenuPermits drop constraint FK_Menus_Permits_PermitId;

--delete constraints
alter table Permits drop constraint UC_Permits_PermitEnum;
alter table Roles drop constraint UC_Roles_RoleName;
alter table Users drop constraint UC_Users_Email;
alter table Users drop constraint UC_Users_NekotNigol;
alter table Logins drop constraint UC_Logins_UserId;
alter table Logins drop constraint UC_Logins_LoginToken;

--drop tables
DROP TABLE RolePermits;
DROP TABLE UserRoles;
DROP TABLE MenuPermits;
DROP TABLE Permits;
DROP TABLE Roles;
DROP TABLE Logins;
DROP TABLE Users;
DROP TABLE Menus;
DROP TABLE CacheKeys;

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

CREATE TABLE Users (
    UserId              int IDENTITY(1,1)   NOT NULL,
    FirstName           varchar(32)         NOT NULL,
    LastName            varchar(32)         NOT NULL,
    Email               varchar(64)         NOT NULL,
    Mobile              varchar(16)         NULL,
    NekotNigol          varchar(128)        NOT NULL,
    StatusFlag          char(1)             NOT NULL,       -- A (active), I (inactive), D (deleted), L (locked), R (archived)
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (UserId ASC),
	CONSTRAINT UC_Users_Email UNIQUE (Email),
    CONSTRAINT UC_Users_NekotNigol UNIQUE (NekotNigol)
);

CREATE TABLE Logins (
    LoginId                 varchar(32)         NOT NULL,
    PasswordHash            varchar(128)        NOT NULL,
    UserId                  int                 NOT NULL,
    LoginToken              varchar(128)        NOT NULL,
    LastLoginDate           datetime            NULL,
    StatusFlag              char(1)             NOT NULL,
    ModifiedBy              int                 NOT NULL,
    ModifiedDate            datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (LoginId ASC),
    CONSTRAINT UC_Logins_UserId UNIQUE (UserId),
    CONSTRAINT UC_Logins_LoginToken UNIQUE (LoginToken),
    CONSTRAINT FK_Logins_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE TABLE RolePermits (
    RolePermitId	    int IDENTITY(1,1)   NOT NULL,
    RoleId		        int                 NOT NULL,
    PermitId	        int                 NOT NULL,
    StatusFlag          char(1)             NOT NULL,
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (RolePermitId ASC),
    CONSTRAINT FK_Roles_Permits_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (RoleId),
    CONSTRAINT FK_Roles_Permits_PermitId FOREIGN KEY (PermitId) REFERENCES Permits (PermitId)
);

CREATE TABLE UserRoles (
    UserRoleId        int IDENTITY(1,1)   NOT NULL,
    UserId            int                 NOT NULL,
    RoleId            int                 NOT NULL,
    StatusFlag        char(1)             NOT NULL,
    ModifiedBy        int                 NOT NULL,
    ModifiedDate      datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (UserRoleId ASC),
    CONSTRAINT FK_Users_Roles_UserId FOREIGN KEY (UserId) REFERENCES Users (UserId),
    CONSTRAINT FK_Users_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (RoleId)
);

CREATE TABLE MenuPermits (
    MenuPermitId      int IDENTITY(1,1)   NOT NULL,
    MenuId            int                 NOT NULL,
    PermitId          int                 NOT NULL,
    StatusFlag        char(1)             NOT NULL,
    ModifiedBy        int                 NOT NULL,
    ModifiedDate      datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (MenuPermitId ASC),
    CONSTRAINT FK_Menus_Permits_MenuId FOREIGN KEY (MenuId) REFERENCES Menus (MenuId),
    CONSTRAINT FK_Menus_Permits_PermitId FOREIGN KEY (PermitId) REFERENCES Permits (PermitId)
);

--users
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('System','sysln','system.email@bpx.com','123-123-1234','nn1','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','devln','developer.email@bpx.com','123-123-1234','nn2','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('QualityAnalyst','qaln','qualityanalyst.email@bpx.com','123-123-1234','nn3','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('Superuser','superuserln','superuser.email@bpx.com','123-123-1234','nn4','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('Admin','admln','admin.email@bpx.com','123-123-1234','nn5','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('Manager','mgrln','manager.email@bpx.com','123-123-1234','nn6','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('fn07','07LN','entity07.email@bpx.com','123-123-1234','nn7','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('fn08','08LN','entity08.email@bpx.com','123-123-1234','nn8','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,NekotNigol,StatusFlag,ModifiedBy,ModifiedDate) values ('fn09','09LN','entity09.email@bpx.com','123-123-1234','nn9','A',1,getDate());

--logins
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (1,'one','AQAAAAEAACcQAAAAEJZUbWun++BJlFSiC352oPAgJ9UKJRXkKX4lD3hbjIsMSx6X+4qVUxkCIqTibouCkg==','token001','A',1,getDate()); 
-- password is: test1111
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (2,'two','AQAAAAEAACcQAAAAEB9JDwdihl7mM0l+TriHpUJ1RMwMBCPEcZyzx0I3jr76qDSCk4BrMXTw04I41QECsA==','token002','A',1,getDate()); 
-- password is: test2222
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (3,'three','AQAAAAEAACcQAAAAEEWfT9/y2LjqdjMDTjtpL6/atZbp0W35CU16ho4ZPc941tdMwWkQkjKmTG2CLpVNWw==','token003','A',1,getDate()); 
-- password is: test3333
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (4,'four','AQAAAAEAACcQAAAAEORU4xK4/14Hif4n9GskVGf0sARqO1Imofs0/hF8petfwQvJIatWXHFttl7fu9JMQw==','token004','A',1,getDate()); 
-- password is: test4444
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (5,'five','AQAAAAEAACcQAAAAEIqxBmeAMdu7VCHVUHdhWwE2gVFMMwAnj4gluKD3jH/7km5lCH24t0UmvVIV6Xldvg==','token005','A',1,getDate()); 
-- password is: test5555
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (6,'six','AQAAAAEAACcQAAAAEE0lGYLrUerJtuTesTovq5x0dYjJbXuHNoWxqc8fplpUatp+ZVGDK5tBO3fdv/KsBQ==','token006','A',1,getDate()); 
-- password is: test6666
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (7,'seven','AQAAAAEAACcQAAAAEGsbNHvJSb009T7pSeRxBrYvOKFrdNhO6eWrMjp6fxw/+eS9Iq7iFKhXW1LubkefEg==','token007','A',1,getDate()); 
-- password is: test7777
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (8,'eight','AQAAAAEAACcQAAAAEEfP/rkD26DdWFjUnEQ/IWyi/6kN8RAZGNcP0n6D+IHgfLX0y37Tm/FrxRX5RaiphQ==','token008','A',1,getDate()); 
-- password is: test8888
insert into Logins (UserId,LoginId,PasswordHash,LoginToken,StatusFlag,ModifiedBy,ModifiedDate) values (9,'nine','AQAAAAEAACcQAAAAEO6wAj4iqxtW6XZW7iCdKmKw3eWaT9nlla1BCdzFd4WfzPeJFyxJga8gBGPrFKXiRg==','token009','A',1,getDate()); 
-- password is: test9999

--roles
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','Developer','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('QualityAnalyst','QualityAnalyst','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Superuser','Superuser','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Admin','Admin','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Manager','Manager','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('RegisterUser','RegisterUser','A',1,getDate());

--permits
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','PermitsGenerator','Index','Developer.Home.Index','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','ErrorLog','Index','Developer.ErrorLog.Index','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','ErrorLog','DownloadLog','Developer.ErrorLog.DownloadLog','A',1,getDate());

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

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Create','Identity.Role.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Read','Identity.Role.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Update','Identity.Role.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Delete','Identity.Role.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','List','Identity.Role.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Filter','Identity.Role.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','ListDeleted','Identity.Role.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Undelete','Identity.Role.Undelete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','RolePermits','Identity.Role.RolePermits','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Create','Identity.Permit.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Read','Identity.Permit.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Update','Identity.Permit.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Delete','Identity.Permit.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','List','Identity.Permit.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Filter','Identity.Permit.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','ListDeleted','Identity.Permit.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Undelete','Identity.Permit.Undelete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','PermitRolesMenus','Identity.Permit.PermitRolesMenus','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Create','Root.Menu.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Read','Root.Menu.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Update','Root.Menu.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Delete','Root.Menu.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','List','Root.Menu.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Filter','Root.Menu.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','ListDeleted','Root.Menu.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','Undelete','Root.Menu.Undelete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Menu','MenuPermits','Root.Menu.MenuPermits','A',1,getDate());


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

--userRoles
insert into UserRoles (UserId,RoleId,StatusFlag,ModifiedBy,ModifiedDate) values ('2','1','A',1,getDate());

--menus
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Home',null,'/',0,0,1,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer',null,'/Developer',1,0,1,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity',null,'/Identity',1,0,2,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Permits Generator',null,'/Developer/PermitsGenerator',2,0,1,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Errors Log',null,'/Developer/ErrorLog',2,0,2,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Menu',null,'/Identity/Menu',3,0,1,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Permit',null,'/Identity/Permit',3,0,2,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('Role',null,'/Identity/Role',3,0,3,null,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,TreePath,StatusFlag,ModifiedBy,ModifiedDate) values ('User',null,'/Identity/User',3,0,4,null,'A',1,getDate());

--MenuPermits
-- none

--cte (common time execution) recursive hierarchy query
WITH cte_menus AS (
	SELECT MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, 1 AS hLevel, OrderNumber, CAST(MenuId AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate  
	FROM Menus WHERE StatusFlag = 'A' AND ParentMenuId = 0
	UNION ALL
	SELECT m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, cte.hLevel + 1, m.OrderNumber, CAST(cte.TreePath + '.' + CAST(m.MenuId AS VARCHAR(32)) AS VARCHAR(32)) AS TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate 
	FROM Menus m INNER JOIN cte_menus cte ON cte.MenuId = m.ParentMenuId WHERE m.StatusFlag = 'A'
)
SELECT MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, CAST('.' + TreePath + '.' AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate FROM cte_menus 
ORDER BY MenuURL