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
       
        /// <summary>
        /// GET Methode Login 
        /// </summary>
        /// <returns></returns>
        // GET: Login
        public ActionResult Login()
        {
            //if (TempData["ConfirmMessage"] != null)
            //{               
            //   TempData["ConfirmMessage"] = "Änderung des Benutzernamens erfolgreich";                                
            //}
            
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

                    TempData["ConfirmMessage"] = "Login erfolgreich";
                    return RedirectToAction("EventDefaultUser", "Events");
                }
                TempData["ErrorMessage"] = "Email existiert nicht!";
                return RedirectToAction("Login", "Login");
            }
            
        }

    }
}