CREATE UNIQUE INDEX email_UNIQUE ON Members (email);
GO

CREATE UNIQUE INDEX memberproperties_UNIQUE ON Memberproperties (member_id,propertytype_id,val);
GO

CREATE UNIQUE INDEX propertytypes_UNIQUE ON Propertytypes ([description]);
GO

CREATE UNIQUE INDEX membervalidations_UNIQUE ON Membervalidation (member_id);
GO

CREATE UNIQUE INDEX membersubscriptions_UNIQUE ON Membersubscriptions (member_id);
GO

CREATE UNIQUE INDEX events_UNIQUE ON [Events] (member_id,eventdate,title,[description],location);
GO

CREATE UNIQUE INDEX eventinvitations_UNIQUE ON Eventinvitations (event_id,email);
GO

CREATE UNIQUE INDEX invitationstatus_UNIQUE ON Invitationstatus (eventinvitations_id);
GO
