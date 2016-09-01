ALTER TABLE Membervalidation
ADD
CONSTRAINT fk_Membervalidation_Member
FOREIGN KEY (member_id)
REFERENCES Members(id);
GO

ALTER TABLE Memberproperties
ADD
CONSTRAINT fk_Memberproperties_Member
FOREIGN KEY (member_id)
REFERENCES Members(id);
GO

ALTER TABLE Memberproperties
ADD
CONSTRAINT fk_Memberproperties_Propertytypes
FOREIGN KEY (propertytype_id)
REFERENCES Propertytypes(id);
GO

ALTER TABLE [Events]
ADD
CONSTRAINT fk_Events_Members
FOREIGN KEY (member_id)
REFERENCES Members(id);
GO

ALTER TABLE Eventinvitations
ADD
CONSTRAINT fk_Eventinvitations_Events
FOREIGN KEY (event_id)
REFERENCES [Events](id);
GO

ALTER TABLE Invitationstatus
ADD
CONSTRAINT fk_Invitationstatus_Eventinvitations
FOREIGN KEY (eventinvitations_id)
REFERENCES [Eventinvitations](id);
GO

ALTER TABLE Membersubscriptions
ADD
CONSTRAINT fk_Membersubscriptions_Member
FOREIGN KEY (member_id)
REFERENCES Members(id);
GO