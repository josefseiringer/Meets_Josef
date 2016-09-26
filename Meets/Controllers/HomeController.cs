using Meets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Meets.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// GET Methode alle Events die Public gesetzt sind anzeigen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()

        {            
            using (MeetsEntities con = new MeetsEntities())
            {
                List<Event> openEvent = (from e in con.Events
                                         join m in con.Members on e.member_id equals m.id
                                         where e.viewpublic == true 
                                         select e).ToList();
                
                if (openEvent == null)
                {
                    ViewBag.keineOffenenBeiträge = "Es sind keine offenen Veranstaltungen vorhanden";
                    return View();
                }
                //Lambda expression zum Sortieren nach Eventdatum von Neu zu alt
                
                openEvent = openEvent.Where(o => o.eventdate > DateTime.Now).OrderByDescending(o => o.eventdate).ToList();
                return View(openEvent);             
            }
            
        }
        


    }
}