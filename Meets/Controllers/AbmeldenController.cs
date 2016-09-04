using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Meets.Controllers
{
    public class AbmeldenController : Controller
    {
        /// <summary>
        /// Methode: [HttpGet]
        /// Methode löscht die Daten der aktuellen Session und ruft den Login-View auf
        /// </summary>
        /// <returns>Zum Login View</returns>
        public ActionResult Logout()
        {

            //Auth-Cookie entfernen, bzw. ausloggen
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Login");
        }
    }
}