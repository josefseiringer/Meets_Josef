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
        public ActionResult Benutzerauswertung()
        {

            List<sp_UserInvitations_Result> result;
            using (MeetsEntities con = new MeetsEntities())
            {
                result = con.sp_UserInvitations().ToList();                
            }
            return View(result);

        }

        [HttpGet]
        [Authorize]
        public ActionResult Delete(int? id)
        {
            //überprüfung ob eine id vorhanden bei null zurück zu Benutzerverwaltung
            if (id == null)
            {
                TempData["ErrorMessage"] = "Uups es ist ein Fehler aufgetreten";
                return RedirectToAction("Benutzerauswertung");
            }

            //ab hier wird gelöscht 
            using (MeetsEntities con = new MeetsEntities())
            {
                //Hole Member anhand der Id
                var getMember = (from me in con.Members
                                 where id == me.id
                                 select me).FirstOrDefault();
                //Setzt auf gelöscht
                getMember.deleted = true;

                //Speichern
                con.SaveChanges();

                TempData["ConfirmMessage"] = "User gelöscht!";

            }
            return RedirectToAction("Benutzerauswertung");
        }


        /// <summary>
        /// GET Methode Zusatzinfos für Dynamische Einträge in Datenbank für Tabelle Memberproperties und Propertytype Anzeigen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Zusatzinfos()
        {
            string defaultUserEmail = User.Identity.Name;
            using (MeetsEntities cont = new MeetsEntities())
            {
                //Entitätssumme Member to List laden über Angemeldete E-Mail
                List<Member> memVal = (from m in cont.Members
                                 where m.email == defaultUserEmail
                                 select m).ToList();
                //Überprüfen ob Entitätsmenge Default Member vorhanden
                if (memVal.Count != 0)
                {
                    //Entitätsmenge aus Memberproperties als Liste für View 
                    List<Memberproperty> memProp = (from mp in cont.Memberproperties
                                              where mp.member_id == mp.Member.id
                                              select mp).ToList();
                    //Überprüfung ob in Memberproperties Einträge existieren
                    if (memProp.Count != 0)
                    {
                        //Mit ViewBag Liste dem View übergeben
                        ViewBag.valMemProp = memProp;

                        //neue Liste vom Type Propertytype
                        List<Propertytype> fertig = new List<Propertytype>();
                        //durch Liste interieren und Daten auf neue Liste speichern
                        foreach (var item in memProp)
                        {
                            //Instanz erzeugen mit pointer pt als Variable
                            Propertytype pt = new Propertytype();
                            //Hole bei jedem Durchgang die Entitäsmenge um die neue Liste zu befüllen
                            Propertytype propType = (from ptt in cont.Propertytypes
                                                     where ptt.id == item.propertytype_id
                                                     select ptt).FirstOrDefault();
                            pt.created = propType.created;
                            pt.description = propType.description;
                            pt.id = propType.id;
                            //pt-Sammlung in Liste ablegen
                            fertig.Add(pt);
                        }

                        cont.SaveChanges();
                        //wenn Liste fertig Inhalt hat 
                        if (fertig.Count != 0)
                        {
                            //Mit VieBag dem View übergeben
                            ViewBag.PropType = fertig;
                        }
                    }                    
                    return View(memVal);
                }               
                //ViewBag.BindingError = "Keine Zusatz Daten vorhanden ";
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

                //instanzen erstellen mit variablen von Zwischentabelle Memberproperties und speichern
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
                //Wenn Id vorhanden dann                
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
            //Controllerseitig Passwort Vergleich prüfen
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
                    //lokale Hilfsvariable 
                    byte[] pasHash = Helper.GetHash(pvm.NewPasswort);
                    
                    //Änderungen der Datenbank übergeben
                    foreach (Member mem in query)
                    {
                        //Update von Passwort
                        mem.password = pasHash;
                    }
                    try
                    {
                        cont.SaveChanges();                       
                        //sendet eine Bestätigungsmail an den User und hat einen Rückgabe string zur weiteren Verwndung wenn der User bestätigt kommt er auf die Login Seite zur Anmeldung
                        string zugangsaenderung = Helper.SendMailRegTo(pvm.Email);
                        //lokale Variable zum Zwischenspeichern des Rückgabewertes aus Methode SendMailRegTo
                        string anhang = null;
                        if (zugangsaenderung != null)
                        {
                            anhang = zugangsaenderung;
                        }
                        //toastermeldung im View EventDefaultUser
                        TempData["ConfirmMessage"] = anhang + " - Änderung des Passwortes erfolgreich";
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
            //Überprüfung des Alters der Benutzer muß 18 Jahre alt sein
            DateTime years18 = DateTime.Now.AddYears(-18);
            if (years18 < rfm.Geburtsdatum)
            {
                //Fehlermeldung an View wenn Benutzer nicht 18
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
                    //Hohle Id aus Datenbank Tabelle Member über Angemeldete E-Mail
                    rfm.IDMember = (from m in con.Members
                                    where rfm.Email == m.email
                                    select m.id).FirstOrDefault();
                }
                string antwort = null;
                //Mailversand an neuen Benutzer mit Übergabe Mailadresse und vorher ermittelte ID aus Datenbank des neuen Benutzers
                antwort = Helper.SendMailRegTo(rfm.Email,rfm.IDMember);
                //Rückmeldung von Mailversand Methode
                ViewBag.Mailversand = antwort;
                //Toaster Meldung registriert aber noch nicht validiert
                TempData["ConfirmMessage"] = "Du wurdest Registriert musst aber noch deine E-Mail bestätigen.";
                return RedirectToAction("Login","Login");
            }
            catch (Exception)
            {
                //Fehlermeldung an View
                ViewBag.ErrorDatabase = "Email schon vergeben.";
                return View("Registrieren");
            }
        }
    }
}