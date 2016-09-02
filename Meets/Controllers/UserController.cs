using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meets.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Web.Security;

namespace Meets.Controllers
{
    public class UserController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Edit()
        {
            string mail = User.Identity.Name;
            using (MeetsEntities cont = new MeetsEntities())
            {

                //Sucht nach der im View übergebenen id in der Datenbank und liefert ein Objekt Member zurück
                Member memVal = (from m in cont.Members
                              where m.email == mail
                              select m).FirstOrDefault();
                if (memVal != null)
                {
                                        
                    return View(memVal);
                }
            }
            return View(ViewBag.errorMsg = "User nicht vorhanden");
        }

        [HttpPost]
        public ActionResult Edit(Member me)
        {
            if (me != null)
            {
                using (MeetsEntities cont = new MeetsEntities())
                {
                    DateTime cr = DateTime.Now;
                    me.created = cr;
                    var vorhanden = (from m in cont.Members
                                     where m.email == me.email &&
                                     m.password == me.password
                                     select m.email).FirstOrDefault();
                    if (vorhanden == null)
                    {
                        
                        cont.Members.Add(me);
                        cont.SaveChanges();
                        return RedirectToAction("Index", "Events");
                    }
                    else
                    {
                        ViewBag.errorEmail = "Gleiche E-Mail wurde verwendet!";
                        return View();
                    }                    
                }
            }
            ViewBag.errdaten = "Kein Update der Person möglich";
            return View();
        }


        // GET: User
        public ActionResult Registrieren()
        {
            return View();
        }
        /// <summary>
        /// Post Methode zum überprüfen des Mindestalters und zur Speicherung in der Datenbank
        /// durch eine Prozedur sp_RegisterUser
        /// </summary>
        /// <param name="rfm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegistrierungSpeichern(RegistrierFormModel rfm)
        {
            DateTime years18 = DateTime.Now.AddYears(-18);
            if (years18 < rfm.Geburtsdatum)
            {
                ViewBag.ErrorMesage = "Leider nicht volljährig";
                return View("Registrieren");
            }
            try
            {
                //Daten in Datenbank der Tabelle Members hizufügen über Store Prozedure
                using (MeetsEntities cont = new MeetsEntities())
                {
                    cont.sp_RegisterUser(rfm.Email, Helper.GetHash(rfm.Passwort), rfm.Geburtsdatum);
                }
                //Mail an neuen Benutzer
                using (MeetsEntities con = new MeetsEntities())
                {
                    rfm.IDMember = (from m in con.Members
                                    where rfm.Email == m.email
                                    select m.id).FirstOrDefault();
                }
                string antwort = null;
                antwort = Helper.SendMailRegTo(rfm.Email,rfm.IDMember);
                ViewBag.Mailversand = antwort;
                return View("Registrieren");
            }
            catch (Exception)
            {

                ViewBag.ErrorDatabase = "Problem mit Datenbankverbindung";
                return View("Registrieren");
            }
        }
    }
}