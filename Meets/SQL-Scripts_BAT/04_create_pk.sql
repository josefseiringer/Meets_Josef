ALTER TABLE Members
ADD
CONSTRAINT pk_members
PRIMARY KEY(id);
GO

ALTER TABLE Membervalidation
ADD
CONSTRAINT pk_membersvalidation
PRIMARY KEY(id);
GO

ALTER TABLE Propertytypes
ADD
CONSTRAINT pk_Propertytypes
PRIMARY KEY(id);
GO

ALTER TABLE Memberproperties
ADD
CONSTRAINT pk_Memberproperties
PRIMARY KEY(id);
GO

ALTER TABLE [Events]
ADD
CONSTRAINT pk_Events
PRIMARY KEY(id);
GO

ALTER TABLE Eventinvitations
ADD
CONSTRAINT pk_Eventinvitations
PRIMARY KEY(id);
GO

ALTER TABLE Membersubscriptions
ADD
CONSTRAINT pk_Membersubscriptions
PRIMARY KEY(id);
GO

ALTER TABLE Invitationstatus
ADD
CONSTRAINT pk_Invitationstatus
PRIMARY KEY(id);
GO

ALTER TABLE Configtables
ADD
CONSTRAINT pk_Configtables
PRIMARY KEY(id);
GO

ALTER TABLE CreditCardMaster
ADD
CONSTRAINT pk_CreditCardMaster
PRIMARY KEY(id);
GO

ALTER TABLE CreditCardVisa
ADD
CONSTRAINT pk_CreditCardVisa
PRIMARY KEY(id);
GO
