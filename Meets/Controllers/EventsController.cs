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

        [Authorize]
        [HttpGet]
        public ActionResult EventDefaultUser()
        {
            string ma = null;
            if (User.Identity.Name != null)
            {                
                if (TempData["mail"] != null)
                {
                    ViewBag.mail = TempData["mail"];                     
                    ma = ViewBag.mail;
                    List < fn_Show_Event_Table_Result > fsetr01 = db.fn_Show_Event_Table(ma).ToList();
                    return View(fsetr01);
                }
                ma = User.Identity.Name;
                List<fn_Show_Event_Table_Result> fsetr02 = db.fn_Show_Event_Table(ma).ToList();
                return View(fsetr02); 
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Events
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                TempData["ConfirmMessage"] = "Login erfolgreich";
                var events = db.Events.Include(x => x.Member);
                string ma = null;
                ma = User.Identity.Name;
                TempData["mail"] = ma;
                return View(events.ToList());
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

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,created,member_id,eventdate,title,description,viewpublic,location")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.member_id = new SelectList(db.Members, "id", "email", @event.member_id);
            //ViewBag.UserSingle = User.Identity.Name;
            //@event.member_id = ViewBag.UserSingle;
            return View(@event);
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

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,created,member_id,eventdate,title,description,viewpublic,location")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.member_id = new SelectList(db.Members, "id", "email", @event.member_id);
            return View(@event);
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
            return RedirectToAction("Index");
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
