using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Meets.Models;
using System.Web.Security;
using System.Diagnostics;

namespace Meets.Controllers
{
    public class EventsController : Controller
    {
        //Datenbankzugriff für alle ActioResult Methoden ohne Using
        private MeetsEntities db = new MeetsEntities();

        [HttpGet]
        [Authorize]
        public ActionResult InviteTo()
        {

            string eingeloggt = User.Identity.Name;
            //int eventInvId = (from evi in db.Eventinvitations
            //                    where evi.email == eingeloggt
            //                    select evi.id).FirstOrDefault();
            List<Event> listInviteTo = (from ev in db.Events
                                        join evi in db.Eventinvitations on ev.id equals evi.event_id
                                        join ivs in db.Invitationstatus on evi.id equals ivs.eventinvitations_id
                                        where evi.email == eingeloggt && ivs.confirm == true
                                        select ev).ToList();

            if (listInviteTo.Count != 0)
            {
                List<InviteToViewModel> ergebnisListe = new List<InviteToViewModel>();
                InviteToViewModel iv;
                foreach (Event ev in listInviteTo)
                {
                    iv = new InviteToViewModel();
                    iv.EventDatum = ev.eventdate;
                    iv.Titel = ev.title;
                    iv.Beschreibung = ev.description;
                    iv.Location = ev.location;
                    iv.emailVon = ev.Member.email;
                    ergebnisListe.Add(iv);
                }
                db.SaveChanges();

                ergebnisListe = ergebnisListe.OrderByDescending(o => o.EventDatum).ToList();
                return View(ergebnisListe);
            }
            ViewBag.noInvite = "Kein Event vorhanden das du angenommen hast und Eingeladen wurdest!";
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult DetailEventBestaetigt()
        {
            string @email = User.Identity.Name;
            List<BesaetigungDetailView> list = db.BesaetigungDetailViews.SqlQuery("select * from dbo.fn_detailViewFromUserEmail('" + @email + "')").ToList();
            if (list.Count != 0)
            {
                return View(list);
            }
            else
            {
                ViewBag.noEntries = "Keine bestätigten Events vorhanden!";
                return View();
            }
        }
        
        [HttpGet]
        public ActionResult VerteilerMailAnnahme(int @id)
        {
            //in Datenbank nachsehn ob ein Eintrag in eventsinvitation_id existiert
            int invit = (from i in db.Invitationstatus
                         join evi in db.Eventinvitations on @id equals evi.event_id
                         where evi.id == i.eventinvitations_id
                         select i.id).FirstOrDefault();

            Event aktuell = (from e in db.Events
                             where e.id == @id
                             select e).FirstOrDefault();
            ViewBag.invit = invit;
            return View(aktuell);
        }
        
        /// <summary>
        /// Bei Annahme oder Ablehnung Status Speichern in die Ivitatiostatus Tabelle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerteilerMailAnnahme(Event ev, string ja,string nein)
        {
            int @eventid = ev.id;
            if (ev != null)
            {
                //Ermitteln der id in den Eventinvitation über die event_id  aus Event
                var idEventInvit = (from ei in db.Eventinvitations
                                    where ei.event_id == ev.id
                                    select ei.id).FirstOrDefault();
               
                //instanz erzeugen und variable ivs zeigt darauf
                Invitationstatu ivs = new Invitationstatu();
                //bei Annehmen eventinvitation_id mit true speichern in den Invitationstatus
                if (ja != null)
                {
                       
                    ivs.created = DateTime.Now;
                    ivs.eventinvitations_id = idEventInvit;
                    ivs.confirm = true;
                    db.Invitationstatus.Add(ivs);
                    db.SaveChanges();

                    TempData["ConfirmMessage"] = "Annahme wurde bestätigt";
                    
                    return RedirectToAction("VerteilerMailAnnahme", @eventid);
                }
                //bei Ablehen eventinvitation_id mit false speichern in den Invitationstatus
                else if (nein != null)
                {
                    ivs.created = DateTime.Now;
                    ivs.eventinvitations_id = idEventInvit;
                    ivs.confirm = false;
                    db.Invitationstatus.Add(ivs);
                    db.SaveChanges();

                    TempData["ConfirmMessage"] = "Du hast abgelehnt";
                    
                    return RedirectToAction("VerteilerMailAnnahme", @eventid);
                }                
            }
            TempData["ErrorMessage"] = "Kein Event geladen";
            
            return RedirectToAction("VerteilerMailAnnahme", @eventid);
        }

        /// <summary>
        /// Ansicht des gesuchten Events zum verteilen
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Verteiler(int id)
        {
            if (User.Identity.Name != null)
            {
                Event aktuell = (from ev in db.Events
                                 where id == ev.id
                                 select ev).FirstOrDefault();
               //ViewBag.errorDoppelteEmail = TempData["errorDoppelteEmail"];
                

                return View(aktuell);
            }
            return View();
        }

        /// <summary>
        /// Speicherung in der Datenbank das eine Einladung verschickt wurde ink. E-Mail an den einzuladenden
        /// </summary>
        /// <param name="vfm">VerteilerFormModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerteilerSender(VerteilerFormModel vfm)
        {
            string mailEventSent = null;
            //string mailTo = null;
            if (vfm != null)
            {
                //suche aktuelles event
                Event aktuell = (from ev in db.Events
                                 where vfm.id == ev.id
                                 select ev).FirstOrDefault();
                //Instanz erzeugen für Speicherung und Sammlung der Daten
                Eventinvitation ei = new Eventinvitation();
                if (aktuell != null)
                {
                    //Umkopieren der Daten
                    //Event ID speichern 
                    ei.event_id = aktuell.id;
                    //E-Mail des Empfängers speichern
                    ei.email = vfm.Email;
                    // Zeitstempel speichern
                    ei.created = DateTime.Now;
                    //Entitätsmenge speichern
                    db.Eventinvitations.Add(ei);
                    try
                    {
                        
                        db.SaveChanges();
                        //Senden an E-Mail Empfänger durch E-Mail und Id
                        mailEventSent = Helper.SendEventToEmail(vfm.Email, vfm.id, vfm.EventTitle);
                    }
                    catch (Exception)
                    {
                        ViewBag.errorDoppelteEmail = "An " + vfm.Email + " wurde diese E-Mail bereits verschickt!";
                        return View(aktuell);
                    }
                }
                //Rückgabenachricht aus Methode SendEventToEmail an TempData übergeben
                TempData["ConfirmMessage"] = mailEventSent;
                return View(aktuell);
            }

            TempData["ErrorMessage"] = "Fehler mit der Datenbankverbindung";
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult OpenInvite()
        {          
            
            //lokale Variable für default user e-mail
            string userMail = User.Identity.Name;
            List<Event> OpenListEvent = (from ev in db.Events
                                         where ev.viewpublic == true && ev.Member.email != User.Identity.Name
                                         select ev).ToList();

            OpenListEvent = OpenListEvent.Where(o => o.eventdate > DateTime.Now).OrderByDescending(o => o.eventdate).ToList();
            return View(OpenListEvent);
        }

        [Authorize]
        [HttpPost]
        public ActionResult OpenInvite(Event ev, string ja, string nein)
        {
            if (ev != null)
            {
                int eventid = ev.id;
                //int eventid = id;
                //Instanz von Eventinvitation erzeugen 
                Eventinvitation evi = new Eventinvitation();

                //instanz erzeugen und variable ivs zeigt darauf
                Invitationstatu ivs = new Invitationstatu();

                ////bei Annehmen eventinvitation_id mit true speichern in den Invitationstatus
                if (ja != null)
                {
                    evi.created = DateTime.Now;
                    evi.email = User.Identity.Name;
                    evi.event_id = eventid;
                    db.Eventinvitations.Add(evi);
                    try
                    {
                        db.SaveChanges();

                        //id aus Eventinvitation holen
                        var idEventInvit = (from ei in db.Eventinvitations
                                            where ei.event_id == eventid
                                            select ei.id).FirstOrDefault();

                        ivs.created = DateTime.Now;
                        ivs.eventinvitations_id = idEventInvit;
                        ivs.confirm = true;
                        db.Invitationstatus.Add(ivs);
                        db.SaveChanges();

                        TempData["ConfirmMessage"] = "Annahme wurde bestätigt";

                        return RedirectToAction("EventDefaultUser");
                    }
                    catch
                    {
                        ViewBag.schonvorhanden = "Dieses Event wurde schon Angenommen/Abgelehnt";
                    }
                }
                //bei Ablehen eventinvitation_id mit false speichern in den Invitationstatus
                else if (nein != null)
                {
                    evi.created = DateTime.Now;
                    evi.email = User.Identity.Name;
                    evi.event_id = eventid;
                    db.Eventinvitations.Add(evi);
                    try
                    {
                        db.SaveChanges();
                        //id aus Eventinvitation holen
                        var idEventInvit = (from ei in db.Eventinvitations
                                            where ei.event_id == eventid
                                            select ei.id).FirstOrDefault();

                        //sammel für speichern in Invitation
                        ivs.created = DateTime.Now;
                        ivs.eventinvitations_id = idEventInvit;
                        ivs.confirm = false;
                        db.Invitationstatus.Add(ivs);
                        db.SaveChanges();

                        TempData["ConfirmMessage"] = "Du hast abgelehnt";

                        return RedirectToAction("EventDefaultUser");

                    }
                    catch (Exception)
                    {

                        ViewBag.schonvorhanden="Dieses Event wurde schon Angenommen/Abgelehnt";
                    }            

                    
                }
            }
            TempData["ErrorMessage"] = "Verbindungsproblem mit der Datenbank";
            return RedirectToAction("EventDefaultUser");            
        }



        /// <summary>
        /// Ansicht Events des gerade angemeldeten Benutzers
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult EventDefaultUser()
        {            
            
            //Mail Adresse aus anderem ActionResult diesem AR übergeben ohne Authentifizierungscookie
            ViewBag.mail = TempData["mail"];
            //Vom ViewBag auf Locale Variable Speichern
            string ma = ViewBag.mail;
            

            if (User.Identity.Name != null)
            {
                //Uneingeschränke Liste : Sucht alle Events des angemeldeten Benutzers und Speichert Sie in eine liste
                List<Event> defUser = (from e in db.Events
                                       where e.Member.id == e.member_id &&
                                       e.Member.email == User.Identity.Name
                                       select e).ToList();
                //Eingeschränkte Liste: Sucht alle Events des angemeldeten Benutzers die private sind und speichert sie in einer Liste
                List<Event> defUserPrivate = (from e in db.Events
                                              where e.Member.id == e.member_id &&
                                              e.Member.email == User.Identity.Name &&
                                              e.viewpublic == false
                                              select e).ToList();
                // liefert die id ob der angemeldete Benutzer in der Membervalidation eingetragen ist
                int valid = (from e in db.Membervalidations
                             where e.Member.id == e.member_id &&
                             e.Member.email == User.Identity.Name
                             select e.id).FirstOrDefault();
                //Wenn die Liste einen count hat dann
                if (defUser.Count == 0)
                {
                    //Meldung noch keine Einträge dem View übergeben
                    ViewBag.leereListe = "Du hast noch keine Einträge erstellt.";
                    return View();
                }
                //Wenn der Benutzer in der Membervalidation eingetragen ist sprich not null nicht null ist
                if (valid != 0)
                {
                    //Lambda expression Liste sortieren nach Eventdatum neu zu alt
                    defUser = defUser.Where(d => d.eventdate >= DateTime.Now).OrderByDescending(d => d.eventdate).ToList();
                    return View(defUser);
                }
                else
                {
                    //Liste sortieren nach Eventdatum neu zu alt
                    defUserPrivate = defUserPrivate.Where(d => d.eventdate >= DateTime.Now).OrderByDescending(d => d.eventdate).ToList();
                    return View(defUserPrivate);
                }
                
            }
            if (ma != null)
            {
                //per gesendeter e-Mail Adresse Seine erstellten Events anzeigen
                List<Event> defUserPerMail = (from e in db.Events
                                              where e.Member.id == e.member_id &&
                                              e.Member.email == ma
                                              select e).ToList();
                //Falls noch kein Event vorhanden dann
                if (defUserPerMail.Count == 0)
                {
                    ViewBag.leereListe = "Sie haben noch keine Einträge erstellt.";
                    return View(defUserPerMail);
                }
            }
            return RedirectToAction("Login", "Login");
        }
        
        /// <summary>
        /// Erzeugen eines Events
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            int valid = (from e in db.Membervalidations
                         where e.Member.id == e.member_id &&
                         e.Member.email == User.Identity.Name
                         select e.id).FirstOrDefault();
            ViewBag.nurPrivate = valid;
            //Dropedown Liste übergeben mit ViewBag an den Create View aus Datenbank Tabelle Members mit id und email
            ViewBag.member_id = new SelectList(db.Members, "id", "email");
            //ViewBag.UserSingle = User.Identity.Name;
            return View();
        }

        /// <summary>
        /// Speichern des Events in der Datenbank
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event e)
        {
            if (ModelState.IsValid)
            {
                
                e.created = DateTime.Now;
                e.member_id = (from ev in db.Events
                            where ev.Member.email == User.Identity.Name
                            select ev.member_id).FirstOrDefault();
                if (e.member_id != 0 && e.description != null && e.eventdate != null && e.location != null && e.title != null)
                {
                    db.Events.Add(e);
                    db.SaveChanges();
                    return RedirectToAction("EventDefaultUser");
                }
                else if (User.Identity.Name != null && e.description != null && e.eventdate != null && e.location != null && e.title != null)
                {
                    e.member_id = (from m in db.Members
                                   where m.email == User.Identity.Name
                                   select m.id).FirstOrDefault();
                    db.Events.Add(e);
                    db.SaveChanges();
                    return RedirectToAction("EventDefaultUser");
                }
                else
                {
                    ViewBag.fehlendeeingabe = "Fehlende oder falsche Eingabe";
                    return View();
                }
            }
            TempData["ErrorMessage"] = "Fehler mit der Datenbankverbindung";
            return View();
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (User.Identity.Name != null)
            {
                //Uneingeschränke Liste : Sucht alle Events des angemeldeten Benutzers und Speichert Sie in eine liste
                Event defUser = (from e in db.Events
                                where id == e.id &&
                                e.Member.email == User.Identity.Name
                                select e).FirstOrDefault();
                //Eingeschränkte Liste: Sucht alle Events des angemeldeten Benutzers die private sind und speichert sie in einer Liste
                Event defUserPrivate = (from e in db.Events
                                        where id == e.id &&
                                        e.Member.email == User.Identity.Name &&
                                        e.viewpublic == false
                                        select e).FirstOrDefault();
                // liefert die id ob der angemeldete Benutzer in der Membervalidation eingetragen ist
                int valid = (from e in db.Membervalidations
                             where e.Member.id == e.member_id &&
                             e.Member.email == User.Identity.Name
                             select e.id).FirstOrDefault();

                if (valid != 0)
                {
                    return View(defUser);
                }
                else
                {
                    ViewBag.noValidUser = valid;
                    return View(defUserPrivate);
                }
            }return RedirectToAction("EventDefaultUser");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNeu(Event e)
        {
            if (e != null)
            {

                
                //Suche in der Datenbank die Reihe/Zeile für das Update anhand der E-Mail
                var query =
                    from mem in db.Events
                    where mem.id == e.id
                    select mem;

                if (query != null)
                {
                    foreach (Event mem in query)
                    {
                        mem.created = DateTime.Now;
                        mem.eventdate = e.eventdate;
                        mem.title = e.title;
                        mem.description = e.description;
                        mem.viewpublic = e.viewpublic;
                        mem.location = e.location;
                    }
                    try
                    {
                        db.SaveChanges();
                        TempData["ConfirmMessage"] = "Event wurde geändert.";
                        return RedirectToAction("EventDefaultUser");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex;
                    }
                }
                TempData["ErrorMessage"] = "Fehler mit der Datenbankverbindung";
                return View();
            }
            return RedirectToAction("EventDefaultUser");
        }

        /// <summary>
        /// Möglichkeit ein bestehendes Event zu löschen
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                /*Äquivalent zu HTTP-Status 400. BadRequest gibt an, dass die Anforderung vom Server nicht interpretiert werden konnte. BadRequest wird gesendet, wenn kein anderer Fehler zutrifft oder der genaue Fehler nicht bekannt bzw. für diesen kein Fehlercode definiert ist.*/
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //gesuchte Event id auf variable speichern
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                //Gibt eine Instanz der HttpNotFoundResult-Klasse zurück
                return HttpNotFound();
            }
            return View(@event);
        }
        
        /// <summary>
        /// POST Methode die das löschen ausführt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Prozedur löscht aus allen abhängigen Tabellen den Eventeintrag anhand der Id
            db.sp_delete_Event(id);                      
            db.SaveChanges();
            return RedirectToAction("EventDefaultUser");
        }

        /// <summary>
        /// Gibt nicht verwaltete Ressourcen und optional verwaltete Ressourcen frei
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
