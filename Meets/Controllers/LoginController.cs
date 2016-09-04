using Meets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Meets.Controllers
{
    public class LoginController : Controller
    {
       
        // GET: Login
        public ActionResult Login()
        {
            if (TempData["ConfirmMessage"] != null)
            {
                TempData["ConfirmMessage"] = "Änderung des Benutzernamens erfolgreich";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginFormModel lfm)
        {
            //Schnittstelle 'uer.Login' von BL        
            int usercorrect = Helper.userLogin(lfm);
            if (usercorrect != 0)
            {
                if (usercorrect == 1)
                {
                    //Login Authentifizierung
                    //mittels [Authorize] kann bei Actionmethoden auf dieses Cookie zugreifen
                    FormsAuthentication.SetAuthCookie(lfm.Email, false);

                    return RedirectToAction("Index", "Events");
                }
                else if (usercorrect == -1)
                {
                    ViewBag.probDB = "Datenbankverbindung fehlgeschlagen";
                }
                             
            }
           ViewBag.noLogin = "Anmeldung fehlgeschlagen Passwort oder E-Mail falsch!";
           return View();            
        }

        //post Controller anders mit LinQ
        [HttpPost]
        public ActionResult Login2(LoginFormModel lfm)
        {
            using(MeetsEntities cont = new MeetsEntities())
            {
                byte[] pwd = Helper.GetHash(lfm.Password);
                var loginVal = (from m in cont.Members
                                where m.email == lfm.Email &&
                                m.password == pwd
                                select m).FirstOrDefault();

                if (loginVal != null)
                {
                    //Login Authentifizierung
                    //mittels [Authorize] kann bei Actionmethoden auf dieses Cookie zugreifen
                    FormsAuthentication.SetAuthCookie(lfm.Email, false);

                    return RedirectToAction("Index", "Events");
                }
                return RedirectToAction("Login", "Login");
            }
            
        }

    }
}