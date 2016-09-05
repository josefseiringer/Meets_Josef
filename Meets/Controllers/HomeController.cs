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
        
        [HttpGet]
        public ActionResult Index()
        {            
            using (MeetsEntities con = new MeetsEntities())
            {
                List<Event> openEvent = (from e in con.Events
                                         join m in con.Members on e.member_id equals m.id
                                         where e.viewpublic == true 
                                         select e).ToList();
                //if (con.View_Event_open.ToList() == null)
                //{
                //    ViewBag.keineOffenenBeiträge = "Es sind keine offenen Veranstaltungen vorhanden";
                //    return View();
                //}
                //else if (con.View_Event.ToList() == null)
                //{
                //    ViewBag.keinePrivatenBeiträge = "Es sind keine privaten Veranstaltungen vorhanden";
                //    return View();
                //}
                // List<View_Event_open> ev = con.View_Event_open.ToList();
                if (openEvent == null)
                {
                    ViewBag.keineOffenenBeiträge = "Es sind keine offenen Veranstaltungen vorhanden";
                    return View();
                }
                return View(openEvent);             
            }
            
        }
        


    }
}