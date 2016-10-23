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
        public ActionResult CallTopUser()
        {
            // aus Model
            CallTopUserViewModel ctvm;
            List<CallTopUserViewModel> listTop = new List<CallTopUserViewModel>();
            //von Datenbank mit prozedur
            List<sp_Call_TopUser_Result> tempTop = db.sp_Call_TopUser().ToList();
            //umspeichern ins model zur weiterverarbeitung

            foreach (sp_Call_TopUser_Result item in tempTop)
            {
                ctvm = new CallTopUserViewModel();
                ctvm.EMail = item.EMail;
                ctvm.EventCount = item.EventCount;
                listTop.Add(ctvm);

            }
            return View(listTop);
        }


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
            return RedirectToAction("Index");

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