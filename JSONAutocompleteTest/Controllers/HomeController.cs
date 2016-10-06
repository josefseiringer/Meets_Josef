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
            var listMember = (from m in db.Members
                              select m).ToList();
            MemberViewModel mvm;
            List<MemberViewModel> memberList = new List<MemberViewModel>();
            foreach (Member m in listMember)
            {
                mvm = new MemberViewModel();
                mvm.Email = m.email;
                mvm.Geburtsdatum = m.dateofbirth;
                memberList.Add(mvm);
            }
            memberList = memberList.OrderBy(m => m.Email).ToList();
            ViewBag.memberListe = memberList;
            return View();  
        }

        [HttpPost]
        public ActionResult Index(MemberViewModel mvm)
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