use Meets;
go

DECLARE @HashThis varbinary(MAX)
SET @HashThis = HASHBYTES('SHA2_512','123user!')
exec dbo.sp_RegisterUser 'j.seiringer@live.at',@HashThis,'1973-25-07'
exec dbo.sp_RegisterUser 'digitalman@a1.net',@HashThis,'1973-25-07'
exec dbo.sp_RegisterUser 'j.seiringer@gmail.com',@HashThis,'1973-25-07'
exec dbo.sp_RegisterUser 'm.seiringer@gmail.com',@HashThis,'1975-22-12';
exec dbo.sp_RegisterUser 'admin@meets.at',@HashThis,'1973-25-07';
GO

INSERT INTO Membervalidation(member_id) VALUES (1);
GO
INSERT INTO Membervalidation(member_id) VALUES (2);
GO
INSERT INTO Membervalidation(member_id) VALUES (3);
GO
INSERT INTO Membervalidation(member_id) VALUES (4);
GO
INSERT INTO Membervalidation(member_id) VALUES (5);
GO


insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(1,'2016-16-12','Wordpress Einführung','Was ist ein CMS und wie installiere ich Wordpress',1,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(1,'2016-23-12','Wordpress fortgeschritten','Arbeiten mit Themes und Styles',1,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(2,'2016-28-12','C#-MVC Einfhrung','Was ist C# und MVC berhaupt',0,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(3,'2016-15-12','C#-MVC fortgeschritten','Der eigene Webauftritt',0,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(1,'2016-24-12','Weihnachten','Weihnachten feiern mit Christbaum',1,'Zuhause');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(1,'2017-7-01','Vorstellungsgespräch','Firma Recom',0,'Gmunden, 4810, Münzfeldstrasse 1, um 10:00 UHR');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(1,'2017-15-05','Geburtstag','Sandra Seiringer hat Geburtstag',3,'Zuhause');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(1,'2016-31-12','Silvesterparty','Silvester mit Freunden',1,'Zuhause');
GO


insert into Meets.dbo.Eventinvitations (email,event_id) values ('j.seiringer@gmail.com',1);
GO
insert into meets.dbo.Eventinvitations (email,event_id) values ('j.seiringer@gmail.com',3);
GO
insert into Meets.dbo.Invitationstatus (eventinvitations_id,confirm) values (1,1);
GO
insert into Meets.dbo.Invitationstatus (eventinvitations_id,confirm) values (2,0);
GO
insert into meets.dbo.Eventinvitations (email,event_id) values ('j.seiringer@gmail.com',2);
GO
insert into Meets.dbo.Invitationstatus (eventinvitations_id,confirm) values (3,0);
GO
