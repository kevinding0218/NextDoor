EXEC ('CREATE SCHEMA [Identity]')
GO

DROP TABLE [Identity].[Users]
GO

CREATE TABLE [Identity].[Users]
(
	[UID] [INT] IDENTITY(1,1) NOT NULL,
	[Email] [NVARCHAR](100) NOT NULL,
	[Role] [NVARCHAR](15) NOT NULL,
    [PasswordHash] NVARCHAR(200) NOT NULL,
	FirstName NVARCHAR(30) NULL,
	MiddleInitial NVARCHAR(10) NULL,
	LastName NVARCHAR(30) NULL,
	--[Active] [BIT] NOT NULL DEFAULT ((1)),
	[LastLogIn] DATETIME NOT NULL,
	[CreatedBy] INT NULL,
	[CreatedOn] DATETIME NULL,
	[LastUpdatedBy] INT NULL,
	[LastUpdatedOn] DATETIME NULL,
	PRIMARY KEY (UID),
);
GO

DROP TABLE [Identity].[RefreshToken]
GO

CREATE TABLE [Identity].[RefreshToken]
(
	[RefreshTokenID] [INT] IDENTITY(1,1) NOT NULL,
	[Uid] [INT] NOT NULL,
    [Token] NVARCHAR(200) NOT NULL,
	[CreatedAt] DATETIME NOT NULL,
	[RevokedAt] DATETIME NULL,
	PRIMARY KEY (RefreshTokenID),
);
GO

INSERT INTO [Identity].[Users] (Email, Role, PasswordHash, LastLogIn)
VALUES ('test@test.com', 'Admin', '123ha89123ma1', GetDate())

SELECT * FROM [Identity].[Users]
SELECT * FROM [Identity].[RefreshToken]