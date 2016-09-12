select ev.eventdate as Eventdatum,
	   ev.title as Eventtitel,
	   evi.email as Email_Empaenger,
	   ivs.confirm as Bestaetigt
from meets.dbo.Members as me
join meets.dbo.Events as ev
	on ev.member_id = me.id
join meets.dbo.Eventinvitations as evi
	on evi.event_id = ev.id
join meets.dbo.Invitationstatus as ivs
	on ivs.eventinvitations_id = evi.id
where me.email = 'j.seiringer@live.at'
