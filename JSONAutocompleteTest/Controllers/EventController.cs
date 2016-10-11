using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JSONAutocompleteTest.Models;

namespace JSONAutocompleteTest.Controllers
{
    public class EventController : Controller
    {
       [HttpGet]
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Index(string email)
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                List<Event> listEventFromUser = (from ev in con.Events
                                                 where ev.Member.email == email
                                                  select ev).ToList();
                
                con.SaveChanges();
                ViewBag.listEventFromUser = listEventFromUser;
                return View();

            }
        }

        public ActionResult PrintViewEvent(int id)
        {
            if (id != 0)
            {
                using (MeetsEntities con = new MeetsEntities())
                {
                    Event defaultEvent = (from ev in con.Events
                                          where ev.id == id
                                          select ev).FirstOrDefault();
                    string email = (from ev in con.Events
                                    where id == ev.id
                                    select ev.Member.email).FirstOrDefault();

                    con.SaveChanges();
                    ViewBag.email = email;
                    return View(defaultEvent);
                }                 
            }
            return View();
        }


        public JsonResult AutoCompleteMember(string term)
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                var result = (from e in con.Members
                              where e.email.ToLower().Contains(term.ToLower())
                              select new { e.email }).Distinct();
                return Json(result, JsonRequestBehavior.AllowGet); 
            }
        }

    }
}