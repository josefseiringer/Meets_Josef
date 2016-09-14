﻿using System;
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
        
        private MeetsEntities db = new MeetsEntities();

        [HttpGet]
        [Authorize]
        public ActionResult DetailEventBestaetigt()
        {
            string @email = User.Identity.Name;
            List<BesaetigungDetailView> list = db.BesaetigungDetailViews.SqlQuery("select * from dbo.fn_detailViewFromUserEmail('" + @email + "')").ToList();

            return View(list);
        }


        [HttpGet]
        public ActionResult VerteilerMailAnnahme(int @id)
        {
            //in Datenbank nachsehn ob ein Eintrag in eventsinvitation_id existiert
            int invit = (from i in db.Invitationstatus
                         where @id == i.eventinvitations_id
                         select i.eventinvitations_id).FirstOrDefault();

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
            if (ev != null)
            {
                //Ermitteln der id in den Eventinvitation über die event_id  aus Event
                var idEventInvit = (from ei in db.Eventinvitations
                                    where ei.event_id == ev.id
                                    select ei.id).FirstOrDefault();
                //instanz erzeugen und variable ivs zeigt darauf
                Invitationstatu ivs = new Invitationstatu();
                ////bei Annehmen eventinvitation_id mit true speichern in den Invitationstatus
                if (ja != null)
                {      
                    ivs.created = DateTime.Now;
                    ivs.eventinvitations_id = idEventInvit;
                    ivs.confirm = true;
                    db.Invitationstatus.Add(ivs);
                    db.SaveChanges();

                    TempData["ConfirmMessage"] = "Annahme wurde bestätigt";
                    //return RedirectToAction("VerteilerMailAnnahme", ev.id);
                    return RedirectToAction("Home", "Index");
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
                    //return RedirectToAction("VerteilerMailAnnahme", ev.id);
                    return RedirectToAction("Home", "Index");
                }                
            }
            TempData["ErrorMessage"] = "Kein Event geladen";
            return RedirectToAction("VerteilerMailAnnahme", ev.id);
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
                
                return View(aktuell);
            }
            return View();
        }

        /// <summary>
        /// Speicherung in der Datenbank das eine Einladung verschickt wurde ink. E-Mail an den einzuladenden
        /// </summary>
        /// <param name="vfm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerteilerSender(VerteilerFormModel vfm)
        {
            string mailEventSent = null;
            //string mailTo = null;
            if (vfm != null)
            {
                //Senden an E-Mail Empfänger durch E-Mail und Id
                mailEventSent = Helper.SendEventToEmail(vfm.Email, vfm.id, vfm.EventTitle);
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
                    db.SaveChanges();

                }
                //Rückgabenachricht aus Methode SendEventToEmail an TempData übergeben
                TempData["ConfirmMessage"] = mailEventSent;
                return View(aktuell);
            }

            TempData["ErrorMessage"] = "Fehler mit der Datenbankverbindung";
            return View();
        }

        /// <summary>
        /// Ansicht Events des gerade angemeldeten Benutzers
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public ActionResult EventDefaultUser()
        {
            TempData["ConfirmMessage"] = "Login erfolgreich";
            ViewBag.mail = TempData["mail"];
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

                if (defUser.Count == 0)
                {
                    ViewBag.leereListe = "Du hast noch keine Einträge erstellt.";
                    return View(defUserPrivate);
                }
                if (valid != 0)
                {
                    return View(defUser);
                }
                else
                {
                    return View(defUserPrivate);
                }
                
            }
            if (ma != null)
            {
                List<Event> defUserPerMail = (from e in db.Events
                                              where e.Member.id == e.member_id &&
                                              e.Member.email == ma
                                              select e).ToList();

                if (defUserPerMail.Count == 0)
                {
                    ViewBag.leereListe = "Sie haben noch keine Einträge erstellt.";
                    return View(defUserPerMail);
                }

                //return View(defUserPerMail);
            }
            return RedirectToAction("Login", "Login");
        }

        /// <summary>
        /// Erstes View der Ansicht der Events des bestimmten Benutzers wird nicht verwendet
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                TempData["ConfirmMessage"] = "Login erfolgreich";
                //var events = db.Events.Include(x => x.Member);
                List<Event> events2 = (from e in db.Events
                               where e.Member.id == e.member_id &&
                               e.Member.email == User.Identity.Name
                               select e).ToList();
                string ma = null;
                ma = User.Identity.Name;
                TempData["mail"] = ma;
                //return View(events.ToList());
                return View(events2);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler {ex.Message}");
                TempData["ErrorMessage"] = "Fehler mit der Datenbankverbindung";
                return View();
            }
        }

        /// <summary>
        /// Mögliche Detail Ansicht wird nicht verwendet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        /// <summary>
        /// Erzeugen eines Events
        /// </summary>
        /// <returns></returns>
        [Authorize]
        // GET: Events/Create
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
                if (e.member_id != 0)
                {                    
                    db.Events.Add(e);
                    db.SaveChanges();
                    return RedirectToAction("EventDefaultUser");
                }
                else if(User.Identity.Name != null)
                {
                    e.member_id = (from m in db.Members
                                   where m.email == User.Identity.Name
                                   select m.id).FirstOrDefault();
                    db.Events.Add(e);
                    db.SaveChanges();
                    return RedirectToAction("EventDefaultUser");
                }
            TempData["ErrorMessage"] = "Fehler mit der Datenbankverbindung";
            return View();                
                
            }
            TempData["ErrorMessage"] = "Fehlende oder falsche Eingabe";
            return View();
        }

        /// <summary>
        /// Bearbeiten oder Update des Events GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        // GET: Events/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Event @event = db.Events.Find(id);
        //    if (@event == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.member_id = new SelectList(db.Members, "id", "email", @event.member_id);
        //    return View(@event);
        //}

        public ActionResult Edit(int? id)
        {
            if (User.Identity.Name != null)
            {
                //Uneingeschränke Liste : Sucht alle Events des angemeldeten Benutzers und Speichert Sie in eine liste
                Event defUser = (from e in db.Events
                                       where e.Member.id == e.member_id &&
                                       e.Member.email == User.Identity.Name
                                       select e).FirstOrDefault();
                //Eingeschränkte Liste: Sucht alle Events des angemeldeten Benutzers die private sind und speichert sie in einer Liste
                Event defUserPrivate = (from e in db.Events
                                              where e.Member.id == e.member_id &&
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


        // POST: Events/Edit/von mir
        /// <summary>
        /// Update vom bestehenden Event wobei das gleiche Event nicht zwei mal abgespeichert werden kann
        /// durch UIX in der Datenbank
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event e)
        {
            try
            {
                if (e != null)
                {

                    e.created = DateTime.Now;
                    e.member_id = (from ev in db.Events
                                   where ev.Member.email == User.Identity.Name
                                   select ev.Member.id).FirstOrDefault();
                    if (e.member_id != 0)
                    {
                        db.Events.Add(e);
                        db.SaveChanges();
                        return RedirectToAction("EventDefaultUser");
                    }
                    TempData["ErrorMessage"] = "Fehler mit der Datenbankverbindung";
                    return View();

                }
                TempData["ErrorMessage"] = "Fehlende oder falsche Eingabe";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Es kann nicht zweimal das selbe Event eingetragen werden! Ausnamefehler der Web Applikation:"+ex;
                return View();
            }     
        }

        /// <summary>
        /// Möglichkeit ein bestehendes Event zu löschen
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        /// <summary>
        /// POST Methode die das löschen ausführt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("EventDefaultUser");
        }

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
