using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JSONAutocompleteTest.Models;

namespace JSONAutocompleteTest.Controllers
{
    
    public class HomeController : Controller
    {
        MeetsEntities db = new MeetsEntities();

        [HttpGet]
        public ActionResult Index()
        {
            MemberViewModel mvm = new MemberViewModel();
            return View(mvm);  
        }

        [HttpPost]
        public ActionResult SaveMember(MemberViewModel mvm)
        {           
            
            Member m = new Member();
            m.created = DateTime.Now;
            m.dateofbirth = mvm.Geburtsdatum;
            m.email = mvm.Email;
            //Klartextpasswort mittels GetHash für Datenbank Haschen
            byte[] tmppw = mvm.GetHash(mvm.Klartextpasswort);
            m.password = tmppw;
            db.Members.Add(m);
            db.SaveChanges();
            return View();

        }

        public JsonResult AutoCompleteMember(string term)
        {
            var result = (from e in db.Members
                          where e.email.ToLower().Contains(term.ToLower())
                          select new { e.email }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }



    }
}