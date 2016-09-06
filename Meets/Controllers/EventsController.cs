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
        
        private MeetsEntities db = new MeetsEntities();

        //[Authorize]
        //[HttpGet]
        //public ActionResult EventDefaultUser()
        //{
        //    string ma = null;
        //    if (User.Identity.Name != null)
        //    {                
        //        if (TempData["mail"] != null)
        //        {
        //            ViewBag.mail = TempData["mail"];                     
        //            ma = ViewBag.mail;
        //            List < fn_Show_Event_Table_Result > fsetr01 = db.fn_Show_Event_Table(ma).ToList();
        //            return View(fsetr01);
        //        }
        //        ma = User.Identity.Name;
        //        List<fn_Show_Event_Table_Result> fsetr02 = db.fn_Show_Event_Table(ma).ToList();
        //        return View(fsetr02); 
        //    }
        //    return RedirectToAction("Login", "Login");
        //}

        // GET: Events

        [Authorize]
        [HttpGet]
        public ActionResult EventDefaultUser()
        {
            TempData["ConfirmMessage"] = "Login erfolgreich";
            ViewBag.mail = TempData["mail"];
            string ma = ViewBag.mail;
            if (User.Identity.Name != null)
            {
                List<Event> defUser = (from e in db.Events
                                       where e.Member.id == e.member_id &&
                                       e.Member.email == User.Identity.Name
                                       select e).ToList();
                if (defUser.Count == 0)
                {
                    ViewBag.leereListe = "Du hast noch keine Einträge erstellt.";
                    return View(defUser);
                }
                return View(defUser);
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
        [Authorize]
        // GET: Events/Create
        public ActionResult Create()
        {
            //Dropedown Liste übergeben mit ViewBag an den Create View aus Datenbank Tabelle Members mit id und email
            ViewBag.member_id = new SelectList(db.Members, "id", "email");
            //ViewBag.UserSingle = User.Identity.Name;
            return View();
        }

        //// POST: Events/Create Generiert
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,created,member_id,eventdate,title,description,viewpublic,location")] Event @event)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Events.Add(@event);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.member_id = new SelectList(db.Members, "id", "email", @event.member_id);
        //    //ViewBag.UserSingle = User.Identity.Name;
        //    //@event.member_id = ViewBag.UserSingle;
        //    return View(@event);
        //}


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

        [Authorize]
        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.member_id = new SelectList(db.Members, "id", "email", @event.member_id);
            return View(@event);
        }

        // POST: Events/Edit/5 generiert
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,created,member_id,eventdate,title,description,viewpublic,location")] Event @event)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(@event).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.member_id = new SelectList(db.Members, "id", "email", @event.member_id);
        //    return View(@event);
        //}

        // POST: Events/Edit/von mir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event e)
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
