CREATE TABLE Members (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  email VARCHAR(255) NOT NULL,
  [password] VARBINARY(MAX) NOT NULL,
  dateofbirth DATETIME2 NOT NULL
);
GO

CREATE TABLE Membervalidation (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  member_id INT NOT NULL
);
GO

CREATE TABLE Membersubscriptions (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  member_id INT NOT NULL,
  [year] INT NOT NULL
);
GO

CREATE TABLE Propertytypes (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  [description] VARCHAR(50) NOT NULL
);
GO

CREATE TABLE Memberproperties (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  member_id INT NOT NULL,
  propertytype_id INT NOT NULL,
  val VARCHAR(50) NOT NULL
);
GO

CREATE TABLE [Events] (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  member_id INT NOT NULL,
  eventdate DATETIME NOT NULL,
  title VARCHAR(50) NOT NULL,
  [description] VARCHAR(255) NOT NULL,
  viewpublic BIT NOT NULL,
  location VARCHAR(255) NOT NULL
);
GO

CREATE TABLE Eventinvitations (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  event_id INT NOT NULL,
  email VARCHAR(255) NOT NULL
);
GO

CREATE TABLE Invitationstatus (
  id INT IDENTITY(1,1) NOT NULL,
  created DATETIME DEFAULT GetDate() NOT NULL,
  eventinvitations_id INT NOT NULL,
  confirm BIT NOT NULL
);
GO

CREATE TABLE Configtables (
  id INT IDENTITY(1,1) NOT NULL,
  [description] VARCHAR(50) NOT NULL,
  vat VARCHAR(50) NOT NULL
);
GO