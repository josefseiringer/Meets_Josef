using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meets.Models;

namespace Meets.Controllers
{
    public class MemberValController : Controller
    {
        /// <summary>
        /// GET Benutzervalidierung 
        /// </summary>
        /// <returns></returns>
        // GET: MemberVal
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST Benutzervalidierung speichern in Datenbank
        /// </summary>
        /// <param name="mvfm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(MembervalidationFormModel mvfm)
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                Membervalidation mv = new Membervalidation();
                //kopieren von Db auf Model
                mv.created = DateTime.Now;
                mv.member_id = mvfm.member_id;
                //Entitätsmenge in Datenbank speichern
                con.Membervalidations.Add(mv);
                con.SaveChanges();
            }
            return View();
        }        
        
        /// <summary>
        /// Aktuelle Validierung überprüfen ob schon Validiert mit Rückmeldung an View über ViewBag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Validate(int id)
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                RegistrierFormModel rfm = new RegistrierFormModel();
                //email anhand der Id ermitteln
                rfm.Email = (from m in con.Members
                             where m.id == id
                             select m.email).FirstOrDefault();
                //id durch Email ermitteln
                rfm.IDMember = (from m in con.Members
                                where m.email == rfm.Email
                                select m.id).FirstOrDefault();

                Membervalidation mfm = new Membervalidation();
                mfm.created = DateTime.Now;
                mfm.member_id = rfm.IDMember;

                int valid = (from m in con.Membervalidations
                             where m.member_id == mfm.member_id
                             select m.id).FirstOrDefault(); 

                if (valid > 0)
                {
                    ViewBag.valErr = "Deine E-Mail ist schon validiert.";
                    return View();
                }
                else
                {
                    con.Membervalidations.Add(mfm);
                    con.SaveChanges();
                    return View();
                   
                }
                
            }            
        }

        [HttpGet]
        public ActionResult ValidateNew(int id)
        {
            using(MeetsEntities con = new MeetsEntities())
            {
                RegistrierFormModel rfm = new RegistrierFormModel();
                rfm.Email = (from m in con.Members
                             where m.id == id
                             select m.email).FirstOrDefault();

                //id durch Email ermitteln
                rfm.IDMember = (from m in con.Members
                                where m.email == rfm.Email
                                select m.id).FirstOrDefault();
                return View(rfm);
            }            
        }

        [HttpPost]
        public ActionResult ValidateNew(int id, string premium, string nopremium)
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                if (id >= 0 && premium != null)
                {
                    string email = (from m in con.Members
                                    where id == m.id
                                    select m.email).FirstOrDefault();
                    TempData["email"] = email;
                    return RedirectToAction("RegCredit");
                }
                else if (id >= 0 && nopremium != null)
                {                   
                    Membervalidation mfm = new Membervalidation();
                    mfm.created = DateTime.Now;
                    mfm.member_id = id;

                    int valid = (from m in con.Membervalidations
                                 where m.member_id == mfm.member_id
                                 select m.id).FirstOrDefault();

                    if (valid > 0)
                    {
                        ViewBag.valErr = "Deine E-Mail ist schon validiert.";
                        return View();
                    }
                    else
                    {
                        con.Membervalidations.Add(mfm);
                        con.SaveChanges();
                        TempData["ConfirmMessage"] = "Sie wurden als Standardkunde validiert";
                        return RedirectToAction("Login","Login");
                    }
                }
                return View();
            }

        }

        [HttpGet]
        public ActionResult RegCredit()
        {
            string email = TempData["email"].ToString();
            CreditcardViewModel cvm = new CreditcardViewModel();
            cvm.Email = email;
            cvm.KartenAuswahl = new List<string>() { "Visakarte","Masterkarte" };
            return View(cvm);
        }

        [HttpPost]
        public ActionResult RegCredit(CreditcardViewModel cvw)
        {
            using(MeetsEntities con = new MeetsEntities())
            {
                //Kartennummer überprüfen
                bool ok = Helper.checkLuhn(cvw.Kartennummer);

                //wenn ok dann
                if (ok)
                {
                    if (cvw.Kartenart == "Masterkarte")
                    {
                        //Kartendaten Hashen und speichern
                        CreditCardMaster ccm = new CreditCardMaster();
                        ccm.created = DateTime.Now;
                        ccm.firstname = cvw.Vorname;
                        ccm.lastname = cvw.Nachname;
                        ccm.memberEmail = cvw.Email;
                        ccm.mastacardnumber = Helper.GetHash(cvw.Kartennummer);
                        ccm.securecode = Helper.GetHash(cvw.Sicherheitscode);
                        ccm.validmonth = cvw.Kartenmonat;
                        ccm.validYear = cvw.Kartenjahr;
                        con.CreditCardMasters.Add(ccm);
                        con.SaveChanges();

                        //in Membervalidation eintragen 
                        Membervalidation mv = new Membervalidation();
                        mv.member_id = (from m in con.Members
                                        where m.email == ccm.memberEmail
                                        select m.id).FirstOrDefault();
                        mv.created = DateTime.Now;
                        con.Membervalidations.Add(mv);
                        con.SaveChanges();

                        //Positivmeldung im Login View
                        TempData["ConfirmMessage"] = "Kartendaten wurden gespeichert du bist jetzt Premiummitglied";
                        return RedirectToAction("Login", "Login");
                    }
                    else if (cvw.Kartenart == "Visakarte")
                    {
                        CreditCardVisa ccv = new CreditCardVisa();
                        ccv.created = DateTime.Now;
                        ccv.firstname = cvw.Vorname;
                        ccv.lastname = cvw.Nachname;
                        ccv.memberEmail = cvw.Email;
                        ccv.visacardnumber = Helper.GetHash(cvw.Kartennummer);
                        ccv.securecode = Helper.GetHash(cvw.Sicherheitscode);
                        ccv.validmonth = cvw.Kartenmonat;
                        ccv.validYear = cvw.Kartenjahr;
                        con.CreditCardVisas.Add(ccv);
                        con.SaveChanges();

                        //in Membervalidation eintragen 
                        Membervalidation mv = new Membervalidation();
                        mv.member_id = (from m in con.Membervalidations
                                        where m.Member.email == ccv.memberEmail
                                        select m.id).FirstOrDefault();
                        mv.created = DateTime.Now;
                        con.Membervalidations.Add(mv);
                        con.SaveChanges();

                        //Positivmeldung im Login View
                        TempData["ConfirmMessage"] = "Kartendaten wurden gespeichert du bist jetzt Premiummitglied";
                        return RedirectToAction("Login", "Login");
                    }                    
                }
                //wenn nicht ok
                TempData["email"] = cvw.Email;
                TempData["ErrorMessage"] = "Kreditkartennummer ist falsch";
                return RedirectToAction("RegCredit");
            }           
        }
    }
}