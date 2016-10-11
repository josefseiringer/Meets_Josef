using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meets.Models;

namespace Meets.Controllers
{
    public class PrintController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult PrintIndex()
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                List<Event> eventListFromUser = 
                            (from e in con.Events
                             where e.Member.email == User.Identity.Name
                             select e).ToList().Where(x=>x.eventdate >= DateTime.Now).OrderByDescending(x=>x.eventdate).ToList();

                return View(eventListFromUser); 
            }
        }
    }
}