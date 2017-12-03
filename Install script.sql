USE MASTER
GO

IF DB_ID(N'SchoolBellDB') IS NULL
BEGIN
	CREATE DATABASE [SchoolBellDB];
END
GO

DECLARE @version_year INT;
SET @version_year = CAST(SUBSTRING(@@VERSION,PATINDEX(N'%20__%',@@VERSION),4) AS INT)

IF @version_year<2012
BEGIN
	EXEC sp_addsrvrolemember @loginame=N'NT AUTHORITY\SYSTEM', @rolename=N'sysadmin'
END
ELSE
BEGIN
	ALTER SERVER ROLE sysadmin ADD MEMBER [NT AUTHORITY\SYSTEM]
END



USE [SchoolBellDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'dbo.Event',N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Event](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Date] [datetime] NOT NULL,
		[StartTime] [time](7) NOT NULL,
		[Description] [nvarchar](250) NULL,
		[AudioLocation] [nvarchar](max) NOT NULL,
		[PlayerLocation] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_dbo.Event] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'dbo.RepeatedSchedule',N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[RepeatedSchedule](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[StartDateTime] [datetime] NOT NULL,
		[Repetition] [int] NOT NULL,
		[Description] [nvarchar](250) NULL,
		[AudioLocation] [nvarchar](max) NOT NULL,
		[PlayerLocation] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_dbo.RepeatedSchedule] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'dbo.Schedule',N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Schedule](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Day] [int] NOT NULL,
		[StartTime] [time](7) NOT NULL,
		[Description] [nvarchar](250) NULL,
		[AudioLocation] [nvarchar](max) NOT NULL,
		[PlayerLocation] [nvarchar](max) NOT NULL,
	 CONSTRAINT [PK_dbo.Schedule] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

GO
