use Meets;
go

DECLARE @HashThis varbinary(MAX)
SET @HashThis = HASHBYTES('SHA2_512','123user!')
exec dbo.sp_RegisterUser 'j.seiringer@live.at',@HashThis,'1973-25-07'
exec dbo.sp_RegisterUser 'digitalman@a1.net',@HashThis,'1973-25-07'
exec dbo.sp_RegisterUser 'j.seiringer@gmail.com',@HashThis,'1973-25-07'
exec dbo.sp_RegisterUser 'm.seiringer@gmail.com',@HashThis,'1975-22-12';
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(1,'2016-16-09','Wordpress Einfhrung','Was ist ein CMS und wie installiere ich Wordpress',1,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(2,'2016-23-09','Wordpress fortgeschritten','Arbeiten mit Themes und Styles',1,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(3,'2016-28-09','C#-MVC Einfhrung','Was ist C# und MVC berhaupt',0,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

insert into [Events] (member_id,eventdate,title,[description],viewpublic,location) values(4,'2016-15-10','C#-MVC fortgeschritten','Der eigene Webauftritt',0,'U3-Zippererstrasse-->Simmeringer Hauptstrasse 58');
GO

INSERT INTO Membervalidation(member_id) VALUES (2);
GO

INSERT INTO Membervalidation(member_id) VALUES (1);
GO

INSERT INTO Membervalidation(member_id) VALUES (3);
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
