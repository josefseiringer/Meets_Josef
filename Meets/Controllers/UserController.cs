﻿using System;
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
                if (memVal != null)
                {
                                        
                    return View(memVal);
                }
            }
            ViewBag.errorMsg = "User nicht vorhanden";
            return View();
        }

        /// <summary>
        /// POST Methode Speichern der Änderung des Benutzers
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Member me)
        {
            if (me != null)
            {
                using (MeetsEntities cont = new MeetsEntities())
                {           
                    me.created = DateTime.Now;
                    me.dateofbirth = (from m in cont.Members
                                      where m.email == User.Identity.Name
                                      select m.dateofbirth).FirstOrDefault();
                    if (me.Klartextpasswort != null)
                    {
                        me.password = Helper.GetHash(me.Klartextpasswort);
                    }
                    else
                    {
                        ViewBag.notPasswd = "Bitte Passwort eingeben!";
                        return View();
                    }
                    //Abfrage ob User vorhanden
                    var vorhanden = (from m in cont.Members
                                     where m.email == me.email
                                     select m.email).FirstOrDefault();
                    if (vorhanden == null)
                    {
                        //Speichern der aktuellen Userdaten 
                        cont.Members.Add(me);
                        cont.SaveChanges();
                        using (MeetsEntities con2 = new MeetsEntities())
                        {
                            //id der geänderten E-mail ermitteln
                            int id = (from m in con2.Members
                                      where me.email == m.email
                                      select m.id).FirstOrDefault();
                            // instanzen erzeugen
                            MembervalidationFormModel mvm = new MembervalidationFormModel();
                            Membervalidation mv = new Membervalidation();
                            //speichern ins Objekt MembervalidationFormModel
                            mvm.created = DateTime.Now;
                            mvm.member_id = id;
                            //umspeichern ins Datenbank Objekt
                            mv.created = mvm.created;
                            mv.member_id = mvm.member_id;
                            //entitätsmenge sammeln und dann auf Datenbank speichern
                            con2.Membervalidations.Add(mv);
                            con2.SaveChanges();
                            //sendet eine Bestätigungsmail an den User und hat einen Rückgabe string zur weiteren Verwndung wenn der User bestätigt kommt er auf die Login seite zur Anmeldung
                            string zugangsaenderung = Helper.SendMailRegTo(me.email);
                            if (zugangsaenderung != null)
                            {
                                TempData["ConfirmMessage"] = zugangsaenderung;
                            }                        
                            return RedirectToAction("Logout", "Abmelden");
                        }                        
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