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
                //Zusatzinfos laden wenn vorhanden und dem View übergeben
                List<Member> memVal = (from m in cont.Members
                                 where m.email == defaultUserEmail
                                 select m).ToList();
                if (memVal != null)
                {
                    List<Memberproperty> memProp = (from mp in cont.Memberproperties
                                              where mp.member_id == mp.Member.id
                                              select mp).ToList();

                    if (memProp!=null)
                    {
                        ViewBag.valMemProp = memProp;
                    }                   
                    //wenn nicht leer
                    if (memProp != null)
                    {
                        //neue Liste vom Type Propertytype
                        List<Propertytype> fertig = new List<Propertytype>();
                        //durch Liste interieren und daten auf neue Liste speichern
                        foreach (var item in memProp)
                        {
                            Propertytype pt = new Propertytype();

                            Propertytype propType = (from ptt in cont.Propertytypes
                                                     where ptt.id == item.propertytype_id
                                                     select ptt).FirstOrDefault();
                            pt.created = propType.created;
                            pt.description = propType.description;
                            pt.id = propType.id;
                            fertig.Add(pt);
                        }
                        
                        if (fertig != null)
                        {
                            ViewBag.PropType = fertig;
                        }                        
                    }
                }               

                if (memVal != null)
                {                   
                    cont.SaveChanges();
                    return View(memVal);
                }
                ViewBag.BindingError = "Keine Datenbankverbindung";
                return View();
            }               
        }

        [HttpPost]
        public ActionResult Zusatzinfos(string value,string description)
        {
            string email = User.Identity.Name;
            using(MeetsEntities cont = new MeetsEntities())
            {
                //Member Id holen und auf member_id in Memberproperties Speichern
                int memValId = (from m in cont.Members
                                 where m.email == email
                                 select m.id).FirstOrDefault();
                //instanzen erstellen mit variablen von Propertytype und speichern
                Propertytype pt = new Propertytype();
                pt.created = DateTime.Now;
                pt.description = description;
                cont.Propertytypes.Add(pt);
                cont.SaveChanges();

                //instanzen erstellen mit variablen von Zwischentabelle Memberproperties un speichern
                Memberproperty mp = new Memberproperty();
                mp.created = DateTime.Now;
                mp.member_id = memValId;
                mp.propertytype_id = pt.id;
                mp.val = value;
                cont.Memberproperties.Add(mp);

                cont.SaveChanges();
                return RedirectToAction("Zusatzinfos");
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
                
                //Instanz von ProfilViewModel erstellen und pvm verweist darauf
                ProfileViewModel pvm = new ProfileViewModel();               

                pvm.Email = memVal.email;                
                pvm.NewPasswort = null;
                pvm.Passwortvergleich = null;                
                
                
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

                        TempData["ConfirmMessage"] = "Änderung des Benutzernamens erfolgreich";
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