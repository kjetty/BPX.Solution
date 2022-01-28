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
alter table Logins drop constraint UC_Logins_UserId;

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
    StatusFlag          char(1)             NOT NULL,       -- A (active), I (inactive), D (deleted), L (locked), R (archived)
    ModifiedBy          int                 NOT NULL,
    ModifiedDate        datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (UserId ASC),
	CONSTRAINT UC_Users_Email UNIQUE (Email)
);

CREATE TABLE Logins (
    LoginId                 varchar(32)         NOT NULL,
    PasswordSalt            varchar(32)         NOT NULL,
    PasswordHash            varchar(128)        NOT NULL,
    UserId                  int                 NOT NULL,
    LoginToken              varchar(128)        NULL,
    LastLoginDate           datetime            NULL,
    StatusFlag              char(1)             NOT NULL,
    ModifiedBy              int                 NOT NULL,
    ModifiedDate            datetime            NOT NULL,
    PRIMARY KEY CLUSTERED (LoginId ASC),
    CONSTRAINT UC_Logins_UserId UNIQUE (UserId),
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
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('System','sysln','system.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','devln','developer.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('QualityAnalyst','qaln','qualityanalyst.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('Superuser','superuserln','superuser.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('Admin','admln','admin.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('Manager','mgrln','manager.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('fn07','07LN','entity07.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('fn08','08LN','entity08.email@bpx.com','123-123-1234','A',1,getDate());
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) values ('fn09','09LN','entity09.email@bpx.com','123-123-1234','A',1,getDate());

insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn010','ln010','em010@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn011','ln011','em011@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn012','ln012','em012@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn013','ln013','em013@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn014','ln014','em014@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn015','ln015','em015@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn016','ln016','em016@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn017','ln017','em017@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn018','ln018','em018@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn019','ln019','em019@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn020','ln020','em020@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn021','ln021','em021@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn022','ln022','em022@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn023','ln023','em023@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn024','ln024','em024@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn025','ln025','em025@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn026','ln026','em026@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn027','ln027','em027@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn028','ln028','em028@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn029','ln029','em029@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn030','ln030','em030@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn031','ln031','em031@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn032','ln032','em032@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn033','ln033','em033@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn034','ln034','em034@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn035','ln035','em035@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn036','ln036','em036@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn037','ln037','em037@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn038','ln038','em038@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn039','ln039','em039@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn040','ln040','em040@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn041','ln041','em041@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn042','ln042','em042@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn043','ln043','em043@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn044','ln044','em044@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn045','ln045','em045@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn046','ln046','em046@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn047','ln047','em047@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn048','ln048','em048@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn049','ln049','em049@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn050','ln050','em050@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn051','ln051','em051@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn052','ln052','em052@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn053','ln053','em053@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn054','ln054','em054@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn055','ln055','em055@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn056','ln056','em056@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn057','ln057','em057@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn058','ln058','em058@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn059','ln059','em059@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn060','ln060','em060@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn061','ln061','em061@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn062','ln062','em062@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn063','ln063','em063@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn064','ln064','em064@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn065','ln065','em065@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn066','ln066','em066@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn067','ln067','em067@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn068','ln068','em068@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn069','ln069','em069@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn070','ln070','em070@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn071','ln071','em071@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn072','ln072','em072@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn073','ln073','em073@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn074','ln074','em074@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn075','ln075','em075@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn076','ln076','em076@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn077','ln077','em077@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn078','ln078','em078@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn079','ln079','em079@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn080','ln080','em080@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn081','ln081','em081@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn082','ln082','em082@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn083','ln083','em083@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn084','ln084','em084@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn085','ln085','em085@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn086','ln086','em086@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn087','ln087','em087@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn088','ln088','em088@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn089','ln089','em089@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn090','ln090','em090@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn091','ln091','em091@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn092','ln092','em092@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn093','ln093','em093@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn094','ln094','em094@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn095','ln095','em095@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn096','ln096','em096@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn097','ln097','em097@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn098','ln098','em098@test.com',NULL,'A','1',GetDate()); 
insert into Users (FirstName,LastName,Email,Mobile,StatusFlag,ModifiedBy,ModifiedDate) VALUES ('fn099','ln099','em099@test.com',NULL,'A','1',GetDate()); 

--logins
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (1,'one','one','AQAAAAEAACcQAAAAEJZUbWun++BJlFSiC352oPAgJ9UKJRXkKX4lD3hbjIsMSx6X+4qVUxkCIqTibouCkg==','A',1,getDate()); --test1111
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (2,'two','two','AQAAAAEAACcQAAAAEB9JDwdihl7mM0l+TriHpUJ1RMwMBCPEcZyzx0I3jr76qDSCk4BrMXTw04I41QECsA==','A',1,getDate()); --test2222
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (3,'three','three','AQAAAAEAACcQAAAAEEWfT9/y2LjqdjMDTjtpL6/atZbp0W35CU16ho4ZPc941tdMwWkQkjKmTG2CLpVNWw==','A',1,getDate()); --test3333
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (4,'four','four','AQAAAAEAACcQAAAAEORU4xK4/14Hif4n9GskVGf0sARqO1Imofs0/hF8petfwQvJIatWXHFttl7fu9JMQw==','A',1,getDate());
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (5,'five','five','AQAAAAEAACcQAAAAEIqxBmeAMdu7VCHVUHdhWwE2gVFMMwAnj4gluKD3jH/7km5lCH24t0UmvVIV6Xldvg==','A',1,getDate());
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (6,'six','six','AQAAAAEAACcQAAAAEE0lGYLrUerJtuTesTovq5x0dYjJbXuHNoWxqc8fplpUatp+ZVGDK5tBO3fdv/KsBQ==','A',1,getDate());
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (7,'seven','seven','AQAAAAEAACcQAAAAEGsbNHvJSb009T7pSeRxBrYvOKFrdNhO6eWrMjp6fxw/+eS9Iq7iFKhXW1LubkefEg==','A',1,getDate());
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (8,'eight','eight','AQAAAAEAACcQAAAAEEfP/rkD26DdWFjUnEQ/IWyi/6kN8RAZGNcP0n6D+IHgfLX0y37Tm/FrxRX5RaiphQ==','A',1,getDate());
insert into Logins (UserId,LoginId,PasswordSalt,PasswordHash,StatusFlag,ModifiedBy,ModifiedDate) values (9,'nine','nine','AQAAAAEAACcQAAAAEO6wAj4iqxtW6XZW7iCdKmKw3eWaT9nlla1BCdzFd4WfzPeJFyxJga8gBGPrFKXiRg==','A',1,getDate());

--roles
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Developer','Developer','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('QualityAnalyst','QualityAnalyst','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Superuser','Superuser','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Admin','Admin','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('Manager','Manager','A',1,getDate());
insert into Roles (RoleName,RoleDescription,StatusFlag,ModifiedBy,ModifiedDate) values ('RegisterUser','RegisterUser','A',1,getDate());

--permits
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Admin','GenerateScripts','PermitConstants','Admin.GenerateScripts.PermitConstants','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Create','Identity.User.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Read','Identity.User.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Update','Identity.User.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Delete','Identity.User.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','List','Identity.User.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Filter','Identity.User.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','ListDeleted','Identity.User.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','User','Restore','Identity.User.Restore','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Create','Identity.Role.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Read','Identity.Role.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Update','Identity.Role.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Delete','Identity.Role.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','List','Identity.Role.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Filter','Identity.Role.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','ListDeleted','Identity.Role.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Role','Restore','Identity.Role.Restore','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Create','Identity.Permit.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Read','Identity.Permit.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Update','Identity.Permit.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Delete','Identity.Permit.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','List','Identity.Permit.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Filter','Identity.Permit.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','ListDeleted','Identity.Permit.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','Permit','Restore','Identity.Permit.Restore','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','UserRole','CRUD','Identity.UserRole.CRUD','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Identity','RolePermit','CRUD','Identity.RolePermit.CRUD','A',1,getDate());

insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','Create','Root.Menu.Create','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','Read','Root.Menu.Read','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','Update','Root.Menu.Update','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','Delete','Root.Menu.Delete','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','List','Root.Menu.List','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','Filter','Root.Menu.Filter','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','ListDeleted','Root.Menu.ListDeleted','A',1,getDate());
insert into Permits (PermitArea,PermitController,PermitName,PermitEnum,StatusFlag,ModifiedBy,ModifiedDate) values ('Root','Menu','Restore','Root.Menu.Restore','A',1,getDate());

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

--userRoles
insert into UserRoles (UserId,RoleId,StatusFlag,ModifiedBy,ModifiedDate) values ('2','1','A',1,getDate());

--menus
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Home',null,'/',0,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 2',null,'/g2',1,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 3',null,'/g3',1,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 4',null,'/g4',1,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 5',null,'/g5',1,0,5,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 6',null,'/g6',1,0,6,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 7',null,'/g7',1,0,7,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 8',null,'/g8',1,0,8,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Group 9',null,'/g9',1,0,9,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 21',null,'/g2/p21',2,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 22',null,'/g2/p22',2,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 23',null,'/g2/p23',2,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 24',null,'/g2/p24',2,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 25',null,'/g2/p25',2,0,5,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 31',null,'/g3/p31',3,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 32',null,'/g3/p32',3,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 33',null,'/g3/p33',3,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 34',null,'/g3/p34',3,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 41',null,'/g4/p41',4,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 42',null,'/g4/p42',4,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 43',null,'/g4/p43',4,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 44',null,'/g4/p44',4,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 45',null,'/g4/p45',4,0,5,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 51',null,'/g5/p51',5,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 52',null,'/g5/p52',5,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 53',null,'/g5/p53',5,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 61',null,'/g6/p61',6,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 62',null,'/g6/p62',6,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 63',null,'/g6/p63',6,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 64',null,'/g6/p64',6,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 65',null,'/g6/p65',6,0,5,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 66',null,'/g6/p66',6,0,6,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 67',null,'/g6/p67',6,0,7,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 68',null,'/g6/p68',6,0,8,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 81',null,'/g8/p81',8,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 82',null,'/g8/p82',8,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 83',null,'/g8/p83',8,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 84',null,'/g8/p84',8,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 85',null,'/g8/p85',8,0,5,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 86',null,'/g8/p86',8,0,6,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 91',null,'/g9/p91',9,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 92',null,'/g9/p92',9,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 93',null,'/g9/p93',9,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 94',null,'/g9/p94',9,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 331',null,'/g3/p33/p331',17,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 332',null,'/g3/p33/p332',17,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 333',null,'/g3/p33/p333',17,0,3,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 334',null,'/g3/p33/p33',17,0,4,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 521',null,'/g5/p52/p521',25,0,1,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 522',null,'/g5/p52/p522',25,0,2,'A',1,getDate());
insert into Menus (MenuName,MenuDescription,MenuURL,ParentMenuId,HLevel,OrderNumber,StatusFlag,ModifiedBy,ModifiedDate) values ('Page 523',null,'/g5/p52/p523',25,0,3,'A',1,getDate());

--MenuPermits
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('3','3','A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('4','9','A',1,getDate());
insert into MenuPermits (MenuId,PermitId,StatusFlag,ModifiedBy,ModifiedDate) values ('5','21','A',1,getDate());
