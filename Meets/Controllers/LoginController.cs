﻿using Meets.Models;
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
       
        /// <summary>
        /// GET Methode Login 
        /// </summary>
        /// <returns></returns>
        // GET: Login
        public ActionResult Login()
        {
                        
            return View();
        }
       
        //post Controller anders mit LinQ
        /// <summary>
        /// POST Methode Login mit erstellen eines Session cookis
        /// </summary>
        /// <param name="lfm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login2(LoginFormModel lfm)
        {
            if (lfm.Email != "admin@meets.at")
            {
                using (MeetsEntities cont = new MeetsEntities())
                {
                    byte[] pwd = Helper.GetHash(lfm.Password);
                    var loginVal = (from m in cont.Members
                                    where m.email == lfm.Email &&
                                    m.password == pwd && m.deleted != true
                                    select m).FirstOrDefault();

                    if (loginVal != null)
                    {
                        //Login Authentifizierung
                        //mittels [Authorize] kann bei Actionmethoden auf dieses Cookie zugreifen
                        FormsAuthentication.SetAuthCookie(lfm.Email, false);

                        TempData["ConfirmMessage"] = "Login erfolgreich";
                        return RedirectToAction("EventDefaultUser", "Events");
                    }
                    TempData["ErrorMessage"] = "Email/Passwort existiert nicht!";
                    return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                using (MeetsEntities cont = new MeetsEntities())
                {
                    byte[] pwd = Helper.GetHash(lfm.Password);
                    var loginVal = (from m in cont.Members
                                    where m.email == lfm.Email &&
                                    m.password == pwd && m.isAdmin == true
                                    select m).FirstOrDefault();

                    if (loginVal != null)
                    {
                        //Login Authentifizierung
                        //mittels [Authorize] kann bei Actionmethoden auf dieses Cookie zugreifen
                        FormsAuthentication.SetAuthCookie(lfm.Email, false);

                        TempData["ConfirmMessage"] = "Login erfolgreich";
                        return RedirectToAction("Benutzerauswertung", "User");
                    }
                    TempData["ErrorMessage"] = "Email/Passwort existiert nicht!";
                    return RedirectToAction("Login", "Login");
                }
            }
            
            
        }

    }
}