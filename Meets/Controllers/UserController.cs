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

        [HttpGet]
        [Authorize]
        public ActionResult Zusatzinfos()
        {
            string defaultUserEmail = User.Identity.Name;
            using (MeetsEntities cont = new MeetsEntities())
            {





                cont.SaveChanges();
                return null;
            }

               
        }
        

        /// <summary>
        /// GET Methode Benutzer Editieren
        /// </summary>
        /// <returns></returns>
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
                //MemberFormModel mfm = new MemberFormModel();
                //mfm.Klartextpasswort = null;
                //mfm.id = memVal.id;
                //mfm.created = memVal.created;
                //mfm.email = memVal.email;
                //mfm.dateofbirth = memVal.dateofbirth;
                //mfm.password = memVal.password;

                ProfileViewModel pvm = new ProfileViewModel();               

                pvm.Email = memVal.email;
                if (pvm.NewPasswort == null && pvm.Passwortvergleich == null)
                {
                    pvm.NewPasswort = null;
                    pvm.Passwortvergleich = null;
                }
                
                
                if (memVal.id != 0)
                {                                        
                    return View(pvm);
                }
            }
            ViewBag.errorMsg = "User nicht vorhanden";
            return View();
        }

        //Test LinkQ Update in Database
        [HttpPost]
        public ActionResult EditTestUpdateLinkQ(ProfileViewModel pvm)
        {
            pvm.Email = User.Identity.Name;
            if (pvm.NewPasswort != pvm.Passwortvergleich)
            {
                ViewBag.equals = "Passwortvergleichsunterschied";
                return View("Edit");
            }

            if (pvm != null)
            {
                using(MeetsEntities cont = new MeetsEntities())
                {
                    //Suche in der Datenbank die Reihe/Zeile für das Update anhand der E-Mail
                    var query =
                        from mem in cont.Members
                        where mem.email == User.Identity.Name
                        select mem;

                    byte[] pasHash = Helper.GetHash(pvm.NewPasswort);
                    
                    //Änderungen der Datenbank übergeben
                    foreach (Member mem in query)
                    {
                        //mem.created = DateTime.Now;
                        mem.password = pasHash;
                    }
                    try
                    {
                        cont.SaveChanges();                       
                        //sendet eine Bestätigungsmail an den User und hat einen Rückgabe string zur weiteren Verwndung wenn der User bestätigt kommt er auf die Login seite zur Anmeldung
                        string zugangsaenderung = Helper.SendMailRegTo(pvm.Email);
                        if (zugangsaenderung != null)
                        {
                            TempData["ConfirmMessage"] = zugangsaenderung;
                        }
                        return RedirectToAction("EventDefaultUser", "Events");
                    }
                    catch (Exception ex)
                    {

                        ViewBag.fehlerUpdate = "kein Update möglich.  "+ex;
                    }
                    
                }
            }
            TempData["ErrorMessage"] = "Fehler Passwort wurde nicht geändert!";
            return RedirectToAction("EventDefaultUser", "Events");
        }


       
        /// <summary>
        /// GET Methode Registrieren eines Benutzers
        /// </summary>
        /// <returns></returns>
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
                TempData["ConfirmMessage"] = "Du wurdest Registriert musst aber noch deine E-Mail bestätigen.";
                return RedirectToAction("Login","Login");
            }
            catch (Exception)
            {

                ViewBag.ErrorDatabase = "Problem mit Datenbankverbindung";
                return View("Registrieren");
            }
        }
    }
}